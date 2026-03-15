//using Application.Interfaces;
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

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer? connectionMultiplexer = null)
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

        //xoa 1 key cu the
        public async Task RemoveCacheResponseAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            await _distributedCache.RemoveAsync(cacheKey);
            var groupName = cacheKey.Split(':')[0];
            var db = _connectionMultiplexer.GetDatabase();
            await db.SetRemoveAsync($"group:{groupName}", cacheKey);
        }

        //xoa toan bo key trong 1 group - ko scan
        public async Task RemoveCacheResponseByGroupAsync(string groupName)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var groupKey = $"Group:{groupName}";

            // Lấy toàn bộ danh sách key trong nhóm ra
            var keys = await db.SetMembersAsync(groupKey);
            if(keys.Length == 0) return; // Nếu nhóm không có key nào thì không cần xóa gì cả

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

        public async Task SetCacheResponseByGroupAsync(string cacheKey, object response, TimeSpan? absoluteExpiry = null, TimeSpan? slidingExpiry = null)
        {
            //check response co du lieu chua
            if (response == null)
            {
                return;
            }

            //viet lai de co the truyen vao thoi gian het han tu ngoai, neu khong truyen thi mac dinh la 5 phut
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(response, settings);

            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiry.HasValue) options.AbsoluteExpirationRelativeToNow = absoluteExpiry; // Cache sẽ hết hạn sau khoảng thời gian tuyệt đối kể từ thời điểm lưu vào cache.
            if (slidingExpiry.HasValue) options.SlidingExpiration = slidingExpiry; // Cache sẽ hết hạn nếu không có truy cập nào đến cache này trong khoảng thời gian trượt kể từ lần truy cập cuối cùng.
            await _distributedCache.SetStringAsync(cacheKey, json, options);
            
            // 2. Lưu key này vào một nhóm để quản lý (Ví dụ lấy tiền tố làm tên nhóm)
            var groupName = cacheKey.Split(':')[0]; // Giả sử cacheKey là Product_123
            var db = _connectionMultiplexer.GetDatabase();
            await db.SetAddAsync($"Group:{groupName}", cacheKey);
        }

        public async Task AddToGroupAsync(string groupKey, string value)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.SetAddAsync(groupKey, value);
        }
    }
}
