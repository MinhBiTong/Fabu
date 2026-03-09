using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut);

        //cache luu theo document, phai dung serialize
        Task<T?> GetCachedResponseAsync<T>(string cacheKey);

        Task RemoveCacheResponseAsync(string pattern);
    }
}
