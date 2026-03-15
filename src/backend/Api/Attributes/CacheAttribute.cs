using Application.Interfaces;
using Domain.Data.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Api.Attributes
{
    //class dong vai tro la 1 constructor va truyen tham so vao controller [Cache(1000)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds = 1000;

        public CacheAttribute(int timeToLiveSeconds = 1000)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        //controller -> goi ham nay de execute cache
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //add singleton tu redisconfiguration
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();

            //xem cache co chua, loi dung middleware k can phai qua nhieu middleware
            //neu k su dung cache thi cho chay ra ngoai k cache nua
            if (!cacheConfiguration.Enabled)
            {
                await next(); //no chay vao trong controller kiem tra xem co dung cache hay k
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCachedResponseAsync<string>(cacheKey);

            //neu co cache thi response tra ve luon  
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }
            //neu no k co cache - goi vao action method controller
            var excutedContext = await next();
            if (excutedContext.Result is OkObjectResult objectResult)
            {
                await cacheService.SetCacheResponseByGroupAsync(cacheKey, response: objectResult.Value, absoluteExpiry: TimeSpan.FromSeconds(_timeToLiveSeconds), slidingExpiry: TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuider = new StringBuilder();
            keyBuider.Append($"{request.Path}"); //lay path sau do lay key
            
            //lay ten cac tham so truyen vao tu controller
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key)) //lay tham so truyen vao trong cac ham o controller
            {
                keyBuider.Append($"|{key}-{value}"); //dua vao chuoi
            }
            return keyBuider.ToString();
        }
    }
}
