using Application.DTOs.Responses;
using Application.Interfaces;
using Application.Models.Recommendations;
using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class RechargePlanRecommendationService : IRechargePlanRecommendationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRechargePlanRecommendationEngine _recommendationEngine;
    private readonly ILogger<RechargePlanRecommendationService> _logger;

    public RechargePlanRecommendationService(
        IUnitOfWork unitOfWork,
        IRechargePlanRecommendationEngine recommendationEngine,
        ILogger<RechargePlanRecommendationService> logger)
    {
        _unitOfWork = unitOfWork;
        _recommendationEngine = recommendationEngine;
        _logger = logger;
    }

    public async Task<ApiResponse<RechargePlanRecommendationResponse>> RecommendForCustomerAsync(
        long customerId,
        int top = 3,
        int recentTransactionLimit = 20,
        CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
        {
            return ApiResponse<RechargePlanRecommendationResponse>.Fail(400, "CustomerId không hợp lệ.");
        }

        top = Math.Clamp(top, 1, 10);
        recentTransactionLimit = Math.Clamp(recentTransactionLimit, 1, 100);

        try
        {
            _logger.LogInformation(
                "Building recharge plan recommendations for customer {CustomerId}. Top: {Top}, RecentLimit: {RecentLimit}",
                customerId,
                top,
                recentTransactionLimit);

            var customer = await _unitOfWork.Customers.GetWithAccountAsync(customerId);
            if (customer is null)
            {
                return ApiResponse<RechargePlanRecommendationResponse>.Fail(404, "Không tìm thấy khách hàng.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var activePlans = await _unitOfWork.RechargePlans.GetActivePlansAsync();
            if (activePlans.Count == 0)
            {
                return ApiResponse<RechargePlanRecommendationResponse>.Fail(404, "Chưa có gói nạp đang hoạt động.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var rechargeTransactions = await _unitOfWork.Transactions.GetRechargeTransactionAsync(customerId);
            var recentSuccessfulRecharges = rechargeTransactions
                .Where(IsSuccessfulRecharge)
                .OrderByDescending(transaction => transaction.CreatedDate)
                .Take(recentTransactionLimit)
                .ToList();

            var profile = BuildCustomerProfile(customer, recentSuccessfulRecharges);
            var context = new RechargePlanRecommendationContext(profile, activePlans, top);
            var recommendations = _recommendationEngine.Recommend(context).ToList();

            var response = new RechargePlanRecommendationResponse
            {
                CustomerId = customer.Id,
                CustomerType = customer.CustomerType,
                Strategy = "RuleBased",
                RechargeCount = profile.RechargeCount,
                TotalRechargeAmount = profile.TotalRechargeAmount,
                AverageRechargeAmount = profile.AverageRechargeAmount,
                LastRechargeAmount = profile.LastRechargeAmount,
                LastRechargeAt = profile.LastRechargeAt,
                GeneratedAt = DateTimeOffset.UtcNow,
                Recommendations = recommendations
            };

            _logger.LogInformation(
                "Generated {Count} recharge plan recommendations for customer {CustomerId}",
                recommendations.Count,
                customerId);

            return ApiResponse<RechargePlanRecommendationResponse>.Success(
                response,
                "Tạo gợi ý gói nạp thành công.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Recharge plan recommendation request was cancelled for customer {CustomerId}",
                customerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to build recharge plan recommendations for customer {CustomerId}",
                customerId);

            return ApiResponse<RechargePlanRecommendationResponse>.Fail(
                500,
                "Không thể tạo gợi ý gói nạp lúc này.");
        }
    }

    private static RechargePlanCustomerProfile BuildCustomerProfile(
        Customer customer,
        IReadOnlyList<Transaction> rechargeTransactions)
    {
        var rechargeAmounts = rechargeTransactions
            .Where(transaction => transaction.Amount > 0)
            .Select(transaction => transaction.Amount)
            .ToList();

        var rechargeCount = rechargeAmounts.Count;
        var totalRechargeAmount = rechargeAmounts.Sum();
        var averageRechargeAmount = rechargeCount == 0
            ? 0
            : totalRechargeAmount / rechargeCount;

        var latestRecharge = rechargeTransactions.FirstOrDefault();
        var mostFrequentRechargeAmount = rechargeTransactions
            .Where(transaction => transaction.Amount > 0)
            .GroupBy(transaction => transaction.Amount)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => group.Key)
            .Select(group => (decimal?)group.Key)
            .FirstOrDefault();

        return new RechargePlanCustomerProfile(
            customer.Id,
            customer.CustomerType,
            rechargeCount,
            totalRechargeAmount,
            averageRechargeAmount,
            latestRecharge?.Amount,
            ResolveRechargeDate(latestRecharge),
            mostFrequentRechargeAmount);
    }

    private static bool IsSuccessfulRecharge(Transaction transaction)
        => transaction.Status == StatusTransaction.Success &&
           transaction.TransactionType.Equals("Recharge", StringComparison.OrdinalIgnoreCase);

    private static DateTimeOffset? ResolveRechargeDate(Transaction? transaction)
    {
        if (transaction is null)
        {
            return null;
        }

        if (transaction.CompletedAt.HasValue)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(transaction.CompletedAt.Value, DateTimeKind.Utc));
        }

        return transaction.CreatedDate;
    }
}
