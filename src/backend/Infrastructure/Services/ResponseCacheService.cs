using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };

        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer? _connectionMultiplexer;
        private readonly ILogger<ResponseCacheService> _logger;

        public ResponseCacheService(
            IDistributedCache distributedCache,
            ILogger<ResponseCacheService> logger,
            IConnectionMultiplexer? connectionMultiplexer = null)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IDatabase? GetRedisDb() => _connectionMultiplexer?.GetDatabase();

        public async Task<T?> GetCachedResponseAsync<T>(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) return default;

            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedResponse))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(cachedResponse);
        }

        public async Task RemoveCacheResponseAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) return;

            await _distributedCache.RemoveAsync(cacheKey);

            var db = GetRedisDb();
            if (db is null) return;

            var defaultGroup = GetDefaultGroupName(cacheKey);
            foreach (var groupKey in BuildGroupKeyCandidates(defaultGroup))
            {
                await db.SetRemoveAsync(groupKey, cacheKey);
            }
        }

        public async Task RemoveCacheResponseByGroupAsync(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return;

            var db = GetRedisDb();
            if (db is null) return;

            foreach (var groupKey in BuildGroupKeyCandidates(groupName).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var keys = await db.SetMembersAsync(groupKey);
                if (keys.Length == 0) continue;

                foreach (var key in keys)
                {
                    await _distributedCache.RemoveAsync(key.ToString());
                }

                await db.KeyDeleteAsync(groupKey);
            }
        }

        public async Task SetCacheResponseByGroupAsync(
            string cacheKey,
            object response,
            TimeSpan? absoluteExpiry = null,
            TimeSpan? slidingExpiry = null)
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || response is null) return;

            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiry.HasValue) options.AbsoluteExpirationRelativeToNow = absoluteExpiry;
            if (slidingExpiry.HasValue) options.SlidingExpiration = slidingExpiry;

            var json = JsonConvert.SerializeObject(response, SerializerSettings);
            await _distributedCache.SetStringAsync(cacheKey, json, options);

            await AddToGroupAsync(GetDefaultGroupName(cacheKey), cacheKey);
        }

        public async Task AddToGroupAsync(string groupKey, string value)
        {
            if (string.IsNullOrWhiteSpace(groupKey) || string.IsNullOrWhiteSpace(value)) return;

            var db = GetRedisDb();
            if (db is null) return;

            await db.SetAddAsync(NormalizeGroupKey(groupKey), value);
        }

        public async Task SetRawStringAsync(string key, string value, TimeSpan expiry)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry
                };
                await _distributedCache.SetStringAsync(key, value, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis error while saving raw key {Key}", key);
                throw;
            }
        }

        public Task<string?> GetRawStringAsync(string key)
        {
            return _distributedCache.GetStringAsync(key);
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || response is null) return;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive
            };

            var serializedResponse = JsonConvert.SerializeObject(response, SerializerSettings);
            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, options);
        }

        private static string GetDefaultGroupName(string cacheKey)
        {
            var parts = cacheKey.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0].StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                return $"{parts[0]}:{parts[1]}";
            }

            return parts.Length >= 2 ? $"{parts[0]}:{parts[1]}" : parts.FirstOrDefault() ?? cacheKey;
        }

        private static string NormalizeGroupKey(string groupName)
        {
            if (groupName.StartsWith("cache:group:", StringComparison.OrdinalIgnoreCase) ||
                groupName.StartsWith("group:", StringComparison.OrdinalIgnoreCase) ||
                groupName.StartsWith("Group:", StringComparison.Ordinal))
            {
                return groupName;
            }

            return $"cache:group:{groupName}";
        }

        private static IEnumerable<string> BuildGroupKeyCandidates(string groupName)
        {
            yield return NormalizeGroupKey(groupName);
            yield return groupName;
            yield return $"Group:{groupName}";
            yield return $"group:{groupName}";
            yield return $"cache:group:{groupName}";
        }
    }
}
