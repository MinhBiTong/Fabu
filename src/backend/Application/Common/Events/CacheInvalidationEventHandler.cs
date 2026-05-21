using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Events;

public sealed class CacheInvalidationEventHandler : INotificationHandler<CacheInvalidationEvent>
{
    private readonly IResponseCacheService _cacheService;
    private readonly ILogger<CacheInvalidationEventHandler> _logger;

    public CacheInvalidationEventHandler(
        IResponseCacheService cacheService,
        ILogger<CacheInvalidationEventHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task Handle(CacheInvalidationEvent notification, CancellationToken cancellationToken)
    {
        foreach (var key in notification.CacheKeys.Where(key => !string.IsNullOrWhiteSpace(key)).Distinct())
        {
            await _cacheService.RemoveCacheResponseAsync(key);
        }

        foreach (var group in notification.CacheGroups.Where(group => !string.IsNullOrWhiteSpace(group)).Distinct())
        {
            await _cacheService.RemoveCacheResponseByGroupAsync(group);
        }

        _logger.LogInformation(
            "Invalidated cache. Reason: {Reason}. Keys: {KeyCount}. Groups: {GroupCount}",
            notification.Reason,
            notification.CacheKeys.Count,
            notification.CacheGroups.Count);
    }
}
