namespace Application.DTOs.Responses;

public sealed class RechargePlanRecommendationResponse
{
    public long CustomerId { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public string Strategy { get; set; } = "RuleBased";
    public int RechargeCount { get; set; }
    public decimal TotalRechargeAmount { get; set; }
    public decimal AverageRechargeAmount { get; set; }
    public decimal? LastRechargeAmount { get; set; }
    public DateTimeOffset? LastRechargeAt { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public List<RechargePlanRecommendationItemResponse> Recommendations { get; set; } = new();
}

public sealed class RechargePlanRecommendationItemResponse
{
    public long PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BonusAmount { get; set; }
    public int? ValidityDays { get; set; }
    public decimal Score { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<string> ReasonDetails { get; set; } = new();
}
