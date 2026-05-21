using Application.Common.Caching;
using Application.Common.CQRS;
using Application.DTOs.Responses;
using Application.Features.RechargePlans.Dtos;

namespace Application.Features.RechargePlans.Queries;

public sealed record GetAllRechargePlansQuery : ICachedQuery<ApiResponse<List<RechargePlanReadDto>>>
{
    public string CacheKey => CacheKeyBuilder.List("recharge-plan");
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetRechargePlanByIdQuery(long Id) : ICachedQuery<ApiResponse<RechargePlanReadDto>>
{
    public string CacheKey => CacheKeyBuilder.Entity("recharge-plan", Id);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetActiveRechargePlansQuery : ICachedQuery<ApiResponse<List<RechargePlanReadDto>>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { active = true });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetRechargePlanByAmountQuery(decimal Amount) : ICachedQuery<ApiResponse<RechargePlanReadDto>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { Amount });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetRechargePlansByPriceRangeQuery(decimal Min, decimal Max) : ICachedQuery<ApiResponse<List<RechargePlanReadDto>>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { Min, Max });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetPopularRechargePlansQuery(int Top) : ICachedQuery<ApiResponse<List<RechargePlanReadDto>>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { Top });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record GetRechargePlansByProviderQuery(string Provider) : ICachedQuery<ApiResponse<List<RechargePlanReadDto>>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { Provider });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}

public sealed record IsRechargePlanActiveQuery(long Id) : ICachedQuery<ApiResponse<bool>>
{
    public string CacheKey => CacheKeyBuilder.Search("recharge-plan", new { Id, active = true });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.RechargePlans };
}
