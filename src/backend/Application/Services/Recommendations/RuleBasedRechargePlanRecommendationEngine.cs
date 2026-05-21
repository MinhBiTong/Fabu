using Application.DTOs.Responses;
using Application.Interfaces;
using Application.Models.Recommendations;
using Domain.Entities;

namespace Application.Services.Recommendations;

public sealed class RuleBasedRechargePlanRecommendationEngine : IRechargePlanRecommendationEngine
{
    private const decimal ClosenessWeight = 45m;
    private const decimal BonusWeight = 20m;
    private const decimal ValidityWeight = 10m;

    public IReadOnlyList<RechargePlanRecommendationItemResponse> Recommend(RechargePlanRecommendationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ActivePlans.Count == 0)
        {
            return Array.Empty<RechargePlanRecommendationItemResponse>();
        }

        var top = Math.Clamp(context.Top, 1, 10);
        var maxBonus = context.ActivePlans.Max(plan => plan.BonusAmount);
        var maxValidityDays = context.ActivePlans
            .Where(plan => plan.ValidityDays.HasValue)
            .Select(plan => plan.ValidityDays!.Value)
            .DefaultIfEmpty(0)
            .Max();

        return context.ActivePlans
            .Select(plan => ScorePlan(plan, context.CustomerProfile, maxBonus, maxValidityDays))
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Amount)
            .Take(top)
            .ToList();
    }

    private static RechargePlanRecommendationItemResponse ScorePlan(
        RechargePlan plan,
        RechargePlanCustomerProfile profile,
        decimal maxBonus,
        int maxValidityDays)
    {
        var reasons = new List<string>();
        var score = profile.RechargeCount == 0
            ? ScoreColdStartPlan(plan, profile, maxBonus, maxValidityDays, reasons)
            : ScorePersonalizedPlan(plan, profile, maxBonus, maxValidityDays, reasons);

        return new RechargePlanRecommendationItemResponse
        {
            PlanId = plan.Id,
            PlanName = plan.PlanName,
            Amount = plan.Amount,
            BonusAmount = plan.BonusAmount,
            ValidityDays = plan.ValidityDays,
            Score = Math.Round(score, 2),
            Reason = reasons.Count == 0
                ? "Gói đang hoạt động và phù hợp với hồ sơ nạp tiền hiện tại."
                : reasons[0],
            ReasonDetails = reasons
        };
    }

    private static decimal ScorePersonalizedPlan(
        RechargePlan plan,
        RechargePlanCustomerProfile profile,
        decimal maxBonus,
        int maxValidityDays,
        List<string> reasons)
    {
        var targetAmount = ResolveTargetAmount(profile);
        var score = CalculateClosenessScore(plan.Amount, targetAmount, ClosenessWeight);

        if (score >= 32m)
        {
            reasons.Add($"Gần với mức nạp quen thuộc khoảng {targetAmount:N0}.");
        }

        if (profile.MostFrequentRechargeAmount.HasValue &&
            plan.Amount == profile.MostFrequentRechargeAmount.Value)
        {
            score += 10m;
            reasons.Add("Trùng với mệnh giá khách hàng nạp thường xuyên nhất.");
        }

        if (plan.Amount > targetAmount && plan.Amount <= targetAmount * 1.35m)
        {
            score += 7m;
            reasons.Add("Gợi ý nâng nhẹ mệnh giá để nhận thêm lợi ích.");
        }

        score += ScoreCustomerTypeFit(plan, profile, targetAmount, reasons);
        score += ScoreBonus(plan, maxBonus, reasons);
        score += ScoreValidity(plan, maxValidityDays, reasons);

        return score;
    }

    private static decimal ScoreColdStartPlan(
        RechargePlan plan,
        RechargePlanCustomerProfile profile,
        decimal maxBonus,
        int maxValidityDays,
        List<string> reasons)
    {
        var score = 30m;

        reasons.Add("Khách hàng chưa có lịch sử nạp, gợi ý dựa trên gói đang hoạt động và ưu đãi.");

        if (IsPrepaid(profile.CustomerType))
        {
            score += 8m;
            reasons.Add("Khách hàng trả trước nên ưu tiên gói dễ bắt đầu.");
        }
        else if (IsPostpaid(profile.CustomerType))
        {
            score += 5m;
            reasons.Add("Khách hàng trả sau nên ưu tiên gói có giá trị sử dụng ổn định.");
        }

        score += ScoreBonus(plan, maxBonus, reasons);
        score += ScoreValidity(plan, maxValidityDays, reasons);

        return score;
    }

    private static decimal ResolveTargetAmount(RechargePlanCustomerProfile profile)
    {
        if (profile.LastRechargeAmount.HasValue && profile.AverageRechargeAmount > 0)
        {
            // Weight recent behavior a little lower than the long-term average to avoid noisy recommendations.
            return profile.AverageRechargeAmount * 0.65m + profile.LastRechargeAmount.Value * 0.35m;
        }

        return profile.LastRechargeAmount ?? profile.AverageRechargeAmount;
    }

    private static decimal CalculateClosenessScore(decimal planAmount, decimal targetAmount, decimal maxScore)
    {
        if (targetAmount <= 0)
        {
            return maxScore / 2m;
        }

        var relativeDistance = Math.Abs(planAmount - targetAmount) / targetAmount;
        var normalized = 1m - Math.Clamp(relativeDistance, 0m, 1m);
        return maxScore * normalized;
    }

    private static decimal ScoreCustomerTypeFit(
        RechargePlan plan,
        RechargePlanCustomerProfile profile,
        decimal targetAmount,
        List<string> reasons)
    {
        if (IsPrepaid(profile.CustomerType) && plan.Amount <= targetAmount * 1.2m)
        {
            reasons.Add("Phù hợp hành vi trả trước, không vượt quá nhiều so với mức nạp quen thuộc.");
            return 8m;
        }

        if (IsPostpaid(profile.CustomerType) && plan.Amount >= targetAmount)
        {
            reasons.Add("Phù hợp khách hàng trả sau, ưu tiên gói có giá trị cao hơn nhu cầu trung bình.");
            return 6m;
        }

        return 0m;
    }

    private static decimal ScoreBonus(RechargePlan plan, decimal maxBonus, List<string> reasons)
    {
        if (maxBonus <= 0 || plan.BonusAmount <= 0)
        {
            return 0m;
        }

        var score = BonusWeight * (plan.BonusAmount / maxBonus);
        if (score >= 8m)
        {
            reasons.Add($"Có ưu đãi thêm {plan.BonusAmount:N0}.");
        }

        return score;
    }

    private static decimal ScoreValidity(RechargePlan plan, int maxValidityDays, List<string> reasons)
    {
        if (maxValidityDays <= 0 || !plan.ValidityDays.HasValue || plan.ValidityDays.Value <= 0)
        {
            return 0m;
        }

        var score = ValidityWeight * plan.ValidityDays.Value / maxValidityDays;
        if (score >= 6m)
        {
            reasons.Add($"Thời hạn sử dụng {plan.ValidityDays.Value} ngày.");
        }

        return score;
    }

    private static bool IsPrepaid(string customerType)
        => customerType.Equals("Prepaid", StringComparison.OrdinalIgnoreCase);

    private static bool IsPostpaid(string customerType)
        => customerType.Equals("Postpaid", StringComparison.OrdinalIgnoreCase);
}
