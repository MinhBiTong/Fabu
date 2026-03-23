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
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds = 1000)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        //controller -> goi ham nay de execute cache
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //add singleton tu redisconfiguration
            var cacheConfig = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();

            //xem cache co chua, loi dung middleware k can phai qua nhieu middleware
            //neu k su dung cache thi cho chay ra ngoai k cache nua
            if (!cacheConfig.Enabled)
            {
                await next();  //no chay vao trong controller kiem tra xem co dung cache hay k
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request, context.ActionArguments);

            var cachedResponse = await cacheService.GetCachedResponseAsync<string>(cacheKey);

            //neu co cache thi response tra ve luon  
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                context.Result = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }

            var executedContext = await next();

            //neu no k co cache - goi vao action method controller
            if (executedContext.Result is OkObjectResult okResult && okResult.Value != null)
            {
                await cacheService.SetCacheResponseByGroupAsync(
                    cacheKey,
                    okResult.Value,
                    absoluteExpiry: TimeSpan.FromSeconds(_timeToLiveSeconds),
                    slidingExpiry: TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request, IDictionary<string, object?> actionArguments)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Method}-{request.Path}");

            // Query string
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}={value}");
            }

            // Body/Arguments (nếu POST/PUT, lấy từ actionArguments nếu có)
            if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put)
            {
                //lay ten cac tham so truyen vao tu controller
                foreach (var (key, value) in actionArguments.OrderBy(x => x.Key))  //lay tham so truyen vao trong cac ham o controller
                {
                    if (value != null)
                        keyBuilder.Append($"|{key}={value}");
                }
            }

            return keyBuilder.ToString();
        }
    }
}
