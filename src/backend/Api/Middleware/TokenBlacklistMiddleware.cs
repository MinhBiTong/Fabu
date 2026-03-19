using Application.Interfaces;
using Infrastructure.Services;

namespace Api.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IResponseCacheService _responseCacheService;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
            //_responseCacheService = responseCacheService;
        }

        //hien sửa, chuyển IResponseCacheService responseCacheService từ contructor xuống InvokeAsync
        public async Task InvokeAsync(HttpContext context, IResponseCacheService responseCacheService)
        {
            // Check for endpoints that require authentication
            if (context.Request.Path.StartsWithSegments("/api") &&
                context.User.Identities?.Any(identity => identity.IsAuthenticated) == true) 
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    //hien comment
                    //var isBlacklisted = await _responseCacheService.GetCachedResponseAsync<bool?>($"blacklist:{token}");
                    var isBlacklisted = await responseCacheService.GetCachedResponseAsync<bool?>($"blacklist:{token}");

                    if (isBlacklisted == true)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is blacklisted.");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
