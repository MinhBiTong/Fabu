using Application.Interfaces;
using Application.Models.AIChatbot;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.AIChatbot;

public sealed class CustomerSupportRagService : ICustomerSupportRagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AIChatbotConfiguration _configuration;
    private readonly ILogger<CustomerSupportRagService> _logger;

    public CustomerSupportRagService(
        IUnitOfWork unitOfWork,
        IOptions<AIChatbotConfiguration> options,
        ILogger<CustomerSupportRagService> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = options.Value;
        _logger = logger;
    }

    public async Task<ChatbotRagContext> BuildContextAsync(
        long? customerId,
        string message,
        CancellationToken cancellationToken = default)
    {
        var context = new ChatbotRagContext
        {
            CustomerId = customerId
        };

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var plans = await _unitOfWork.RechargePlans.GetActivePlansAsync();
            context.ActiveRechargePlans = plans
                .Take(Math.Max(1, _configuration.MaxActivePlans))
                .Select(FormatRechargePlan)
                .ToList();
            context.RetrievedSources.Add("RechargePlan");

            if (!customerId.HasValue || customerId.Value <= 0)
            {
                return context;
            }

            var customer = await _unitOfWork.Customers.GetWithAccountAsync(customerId.Value);
            if (customer is null)
            {
                _logger.LogInformation("No customer found while building chatbot RAG context. CustomerId: {CustomerId}", customerId);
                return context;
            }

            context.CustomerSummary = FormatCustomer(customer);
            context.RetrievedSources.Add("Customer");

            if (customer.Account is not null)
            {
                context.AccountBalance = customer.Account.Balance;
                context.CreditLimit = customer.Account.CreditLimit;
                context.AccountSummary = FormatAccount(customer.Account);
                context.RetrievedSources.Add("Account");
            }

            var recentTransactions = await _unitOfWork.Transactions.GetRecentTransactionsAsync(
                customerId.Value,
                Math.Max(1, _configuration.MaxRecentTransactions));

            context.RecentTransactions = recentTransactions
                .Select(FormatTransaction)
                .ToList();

            if (context.RecentTransactions.Count > 0)
            {
                context.RetrievedSources.Add("Transaction");
            }

            var unpaidBills = await _unitOfWork.PostpaidBills.GetUnpaidBillsByCustomerAsync(customerId.Value);
            context.UnpaidBills = unpaidBills
                .OrderBy(bill => bill.DueDate)
                .Take(5)
                .Select(FormatPostpaidBill)
                .ToList();

            if (context.UnpaidBills.Count > 0)
            {
                context.RetrievedSources.Add("PostpaidBill");
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build chatbot RAG context for customer {CustomerId}", customerId);
            throw;
        }

        return context;
    }

    private static string FormatRechargePlan(RechargePlan plan)
        => $"ID {plan.Id}: {plan.PlanName}, giá {plan.Amount:N0}, bonus {plan.BonusAmount:N0}, hạn dùng {(plan.ValidityDays?.ToString() ?? "không giới hạn")} ngày, mô tả: {plan.Description}";

    private static string FormatCustomer(Customer customer)
        => $"ID {customer.Id}, tên {customer.FullName}, số thuê bao {customer.MobileNumber}, loại {customer.CustomerType}";

    private static string FormatAccount(Account account)
        => $"số dư {account.Balance:N0}, hạn mức tín dụng {account.CreditLimit:N0}, trạng thái {account.Status}, lần nạp gần nhất {(account.LastRechargeDate?.ToString("yyyy-MM-dd HH:mm") ?? "chưa có")}";

    private static string FormatTransaction(Transaction transaction)
        => $"mã {transaction.TransactionRef}, loại {transaction.TransactionType}, số tiền {transaction.Amount:N0}, trạng thái {transaction.Status}, phương thức {transaction.PaymentMethod}, ngày {(transaction.CompletedAt ?? transaction.CreatedDate.UtcDateTime):yyyy-MM-dd HH:mm}";

    private static string FormatPostpaidBill(PostpaidBill bill)
    {
        var remaining = Math.Max(0, bill.TotalAmount - bill.PaidAmount);
        return $"hóa đơn {bill.Id}, tháng {bill.BillMonth:yyyy-MM}, tổng {bill.TotalAmount:N0}, đã trả {bill.PaidAmount:N0}, còn {remaining:N0}, hạn {bill.DueDate:yyyy-MM-dd}, trạng thái {bill.Status}";
    }
}
