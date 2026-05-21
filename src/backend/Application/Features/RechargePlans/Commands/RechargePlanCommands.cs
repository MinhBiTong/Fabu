using Application.Common.Caching;
using Application.Common.CQRS;
using Application.Common.Events;
using Application.DTOs.Responses;
using Application.Features.RechargePlans.Dtos;

namespace Application.Features.RechargePlans.Commands;

public sealed record CreateRechargePlanCommand(
    string PlanName,
    decimal Amount,
    decimal BonusAmount,
    int? ValidityDays,
    string? Description,
    bool IsActive = true) : ICacheInvalidatingCommand<ApiResponse<RechargePlanReadDto>>
{
    public CacheInvalidationEvent BuildCacheInvalidationEvent(ApiResponse<RechargePlanReadDto> response)
        => new(
            Array.Empty<string>(),
            new[] { CacheGroups.RechargePlans },
            nameof(CreateRechargePlanCommand));
}

public sealed record UpdateRechargePlanCommand(
    long Id,
    string PlanName,
    decimal Amount,
    decimal BonusAmount,
    int? ValidityDays,
    string? Description,
    bool IsActive) : ICacheInvalidatingCommand<ApiResponse<bool>>
{
    public CacheInvalidationEvent BuildCacheInvalidationEvent(ApiResponse<bool> response)
        => new(
            new[] { CacheKeyBuilder.Entity("recharge-plan", Id) },
            new[] { CacheGroups.RechargePlans },
            nameof(UpdateRechargePlanCommand));
}

public sealed record DeleteRechargePlanCommand(long Id) : ICacheInvalidatingCommand<ApiResponse<bool>>
{
    public CacheInvalidationEvent BuildCacheInvalidationEvent(ApiResponse<bool> response)
        => new(
            new[] { CacheKeyBuilder.Entity("recharge-plan", Id) },
            new[] { CacheGroups.RechargePlans },
            nameof(DeleteRechargePlanCommand));
}
