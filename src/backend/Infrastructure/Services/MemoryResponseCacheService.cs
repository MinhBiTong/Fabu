using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class MemoryResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _cache;

        public MemoryResponseCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetCachedResponseAsync<T>(string cacheKey)
        {
            var cached = await _cache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cached)
                ? default
                : JsonConvert.DeserializeObject<T>(cached);
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            var data = JsonConvert.SerializeObject(response);
            await _cache.SetStringAsync(cacheKey, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut
            });
        }

        // thêm method thiếu
        public Task SetCacheResponseByGroupAsync(string key, object value, TimeSpan? slidingExpiration, TimeSpan? absoluteExpiration)
        {
            // có thể bỏ trống hoặc gọi lại SetCacheResponseAsync
            return Task.CompletedTask;
        }

        public Task AddToGroupAsync(string group, string key)
        {
            // Redis version dùng Set để quản lý group
            // Memory version có thể bỏ qua
            return Task.CompletedTask;
        }

        public Task RemoveCacheResponseAsync(string pattern)
        {
            return Task.CompletedTask;
        }

        public Task RemoveCacheResponseByGroupAsync(string groupName)
        {
            return Task.CompletedTask;
        }
    }
}