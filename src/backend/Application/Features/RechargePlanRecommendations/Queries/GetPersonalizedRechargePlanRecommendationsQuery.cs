using Application.Common.Caching;
using Application.Common.CQRS;
using Application.DTOs.Responses;

namespace Application.Features.RechargePlanRecommendations.Queries;

public sealed record GetPersonalizedRechargePlanRecommendationsQuery(
    long CustomerId,
    int Top = 3,
    int RecentTransactionLimit = 20) : ICachedQuery<ApiResponse<RechargePlanRecommendationResponse>>
{
    public string CacheKey => CacheKeyBuilder.Search(
        "recharge-plan-recommendation",
        new { CustomerId, Top, RecentTransactionLimit });

    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);

    public IReadOnlyCollection<string> CacheGroups => new[]
    {
        Application.Common.Caching.CacheGroups.RechargePlans,
        Application.Common.Caching.CacheGroups.Transactions
    };
}
