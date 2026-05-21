using Application.Common.Caching;
using Application.Common.CQRS;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IResponseCacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IResponseCacheService cacheService,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery<TResponse> cachedQuery ||
            string.IsNullOrWhiteSpace(cachedQuery.CacheKey))
        {
            return await next();
        }

        TResponse? cachedResponse = default;
        try
        {
            cachedResponse = await _cacheService.GetCachedResponseAsync<TResponse>(cachedQuery.CacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache read failed for key {CacheKey}", cachedQuery.CacheKey);
        }

        if (cachedResponse is not null)
        {
            _logger.LogInformation("Cache hit for query {QueryName}. Key: {CacheKey}", typeof(TRequest).Name, cachedQuery.CacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for query {QueryName}. Key: {CacheKey}", typeof(TRequest).Name, cachedQuery.CacheKey);
        var response = await next();

        try
        {
            var ttl = cachedQuery.CacheDuration ?? CacheDefaults.QueryTtl;
            await _cacheService.SetCacheResponseAsync(cachedQuery.CacheKey, response!, ttl);

            foreach (var cacheGroup in cachedQuery.CacheGroups.Where(group => !string.IsNullOrWhiteSpace(group)).Distinct())
            {
                await _cacheService.AddToGroupAsync(cacheGroup, cachedQuery.CacheKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache write failed for key {CacheKey}", cachedQuery.CacheKey);
        }

        return response;
    }
}
