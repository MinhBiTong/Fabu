using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        public readonly IDistributedCache _distributedCache;

        public readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T?> GetCachedResponseAsync<T>(string cacheKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedResponse)) return default;

            // Deserialize the JSON string using JsonConvert.DeserializeObject
            return JsonConvert.DeserializeObject<T>(cachedResponse);
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            await foreach (var key in GetKeyAsync(pattern + "*")) //xoa tat ca cac duoi dang sau, chi giu lai prefix o controller
            {
                await _distributedCache.RemoveAsync(key.ToString()); 
            }
        }

        public async Task RemoveCacheResponseByGroupAsync(string groupName)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var groupKey = $"Group:{groupName}";

            // Lấy toàn bộ danh sách key trong nhóm ra
            var keys = await db.SetMembersAsync(groupKey);

            foreach (var key in keys)
            {
                await _distributedCache.RemoveAsync(key.ToString());
            }

            // Xóa luôn cái Set quản lý nhóm đó
            await db.KeyDeleteAsync(groupKey);
        }

        private async IAsyncEnumerable<string> GetKeyAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);

                //Thêm pageSize để thư viện sử dụng SCAN thay vì KEYS
                // pageSize: 1000 nghĩa là mỗi lần quét Redis sẽ lấy ra 1000 key, 
                // sau đó nhường cho các tiến trình khác rồi mới quét tiếp.
                foreach (var key in server.Keys(pattern: pattern, pageSize: 1000))
                {
                    yield return key.ToString();
                }
            }
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            //check response co du lieu chua
            if (response == null)
            {
                return;
            }

            var serializerResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            //luu data vao cache 
            await _distributedCache.SetStringAsync(cacheKey, serializerResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut
            });
            // 2. Lưu key này vào một nhóm để quản lý (Ví dụ lấy tiền tố làm tên nhóm)
            var groupName = cacheKey.Split('_')[0]; // Giả sử cacheKey là Product_123
            var db = _connectionMultiplexer.GetDatabase();
            await db.SetAddAsync($"Group:{groupName}", cacheKey);
        }
    }
}
