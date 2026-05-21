using Application.Common.Security;
using Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenBlacklistMiddleware> _logger;

        public TokenBlacklistMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<TokenBlacklistMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var responseCacheService = context.RequestServices.GetRequiredService<IResponseCacheService>();

            if (context.Request.Path.StartsWithSegments("/api") &&
                context.User.Identities?.Any(identity => identity.IsAuthenticated) == true)
            {
                var accessCookieName = _configuration["AuthSecurity:AccessTokenCookieName"] ?? "fabu_at";
                var token = ExtractAccessToken(context, accessCookieName);
                var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(jti) && !string.IsNullOrWhiteSpace(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        jti = handler.ReadJwtToken(token)
                            .Claims
                            .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)
                            ?.Value;
                    }
                }

                if (string.IsNullOrWhiteSpace(jti) && !string.IsNullOrWhiteSpace(token))
                {
                    jti = AuthCacheKeys.Sha256(token);
                }

                if (!string.IsNullOrWhiteSpace(jti))
                {
                    var isBlacklisted = await responseCacheService
                        .GetCachedResponseAsync<bool?>(AuthCacheKeys.AccessTokenBlacklist(jti));

                    if (isBlacklisted != true && !string.IsNullOrWhiteSpace(token))
                    {
                        isBlacklisted = await responseCacheService
                            .GetCachedResponseAsync<bool?>($"blacklist:{token}");
                    }

                    if (isBlacklisted == true)
                    {
                        _logger.LogWarning("Blocked blacklisted access token. Jti: {Jti}", jti);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is blacklisted.");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private static string ExtractAccessToken(HttpContext context, string accessCookieName)
        {
            var bearerToken = context.Request.Headers.Authorization
                .ToString()
                .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                return bearerToken;
            }

            return context.Request.Cookies[accessCookieName] ?? string.Empty;
        }
    }
}
