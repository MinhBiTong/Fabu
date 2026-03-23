using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseByGroupAsync(string cacheKey, object response, TimeSpan? absoluteExpiry = null, TimeSpan? slidingExpiry = null);

        //cache luu theo document, phai dung serialize
        Task<T?> GetCachedResponseAsync<T>(string cacheKey);

        Task RemoveCacheResponseAsync(string pattern);
        Task RemoveCacheResponseByGroupAsync(string pattern);
        Task AddToGroupAsync(string groupKey, string value);
    }
}
