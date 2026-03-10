using Application.Interfaces;

namespace Api.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IResponseCacheService _responseCacheService;

        public TokenBlacklistMiddleware(RequestDelegate next, IResponseCacheService responseCacheService)
        {
            _next = next;
            _responseCacheService = responseCacheService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check for endpoints that require authentication
            if (context.Request.Path.StartsWithSegments("/api") &&
                context.User.Identities?.Any(identity => identity.IsAuthenticated) == true) 
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    var isBlacklisted = await _responseCacheService.GetCachedResponseAsync<bool?>($"blacklist:{token}");
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
