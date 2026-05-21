using Domain.Entities;

namespace Application.Models.Recommendations;

public sealed record RechargePlanRecommendationContext(
    RechargePlanCustomerProfile CustomerProfile,
    IReadOnlyList<RechargePlan> ActivePlans,
    int Top);

public sealed record RechargePlanCustomerProfile(
    long CustomerId,
    string CustomerType,
    int RechargeCount,
    decimal TotalRechargeAmount,
    decimal AverageRechargeAmount,
    decimal? LastRechargeAmount,
    DateTimeOffset? LastRechargeAt,
    decimal? MostFrequentRechargeAmount);
