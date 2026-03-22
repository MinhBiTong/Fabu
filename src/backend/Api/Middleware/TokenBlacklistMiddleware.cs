using Application.Interfaces;

namespace Api.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenBlacklistMiddleware> _logger;

        public TokenBlacklistMiddleware(RequestDelegate next, ILogger<TokenBlacklistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var responseCacheService = context.RequestServices
                .GetRequiredService<IResponseCacheService>();

            if (context.Request.Path.StartsWithSegments("/api") &&
                context.User.Identities?.Any(i => i.IsAuthenticated) == true)
            {
                var token = context.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "")
                    .Trim();

                if (!string.IsNullOrEmpty(token))
                {
                    var isBlacklisted = await responseCacheService
                        .GetCachedResponseAsync<bool?>($"blacklist:{token}");

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
