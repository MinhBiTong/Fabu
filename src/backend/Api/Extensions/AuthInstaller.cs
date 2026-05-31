using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace Api.Extensions
{
    public class AuthInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var authConfig = configuration.GetSection("Auth");
            if (!authConfig.GetValue<bool>("Enabled")) return;

            var jwtSettings = configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured in the application settings.");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            var accessTokenCookieName = configuration["AuthSecurity:AccessTokenCookieName"] ?? "fabu_at";
            var cookieSecure = configuration.GetValue("AuthSecurity:CookieSecure", true);

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies", options =>
            {
                options.Cookie.Name = "fabu.external";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = cookieSecure
                    ? CookieSecurePolicy.Always
                    : CookieSecurePolicy.SameAsRequest;
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrWhiteSpace(context.Token) &&
                            context.Request.Cookies.TryGetValue(accessTokenCookieName, out var cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        if (!authConfig.GetValue<bool>("CheckBanEnabled"))
                        {
                            return;
                        }

                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (userId is null)
                        {
                            throw new AppException(ErrorCode.UNAUTHENTICATED);
                        }

                        var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
                        if (userManager != null)
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user is null || !user.IsActive)
                            {
                                throw new AppException(ErrorCode.UNAUTHENTICATED);
                            }
                        }
                    }
                };
            });

            AddGoogleProvider(authenticationBuilder, configuration);
            AddGitHubProvider(authenticationBuilder, configuration);
            AddOidcProvider(authenticationBuilder, configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserDelete", policy => policy.RequireClaim("permission", "user.delete"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CanEditPost", policy => policy.RequireAssertion(context =>
                    HasRole(context.User, "Admin") ||
                    HasPermission(context.User, "post.edit", "post.update", "write:all")));
                options.AddPolicy("CanManageUsers", policy => policy.RequireAssertion(context =>
                    HasRole(context.User, "Admin") ||
                    HasPermission(context.User, "user.create", "user.update", "user.delete", "write:all")));
                options.AddPolicy("CanManagePayments", policy => policy.RequireAssertion(context =>
                    HasRole(context.User, "Admin") ||
                    HasPermission(context.User, "payment.manage", "write:all")));
                options.AddPolicy("CanViewAuditLogs", policy => policy.RequireAssertion(context =>
                    HasRole(context.User, "Admin") ||
                    HasPermission(context.User, "system.audit.read", "read:all")));
                options.AddPolicy("RequireScope", policy => policy.RequireAssertion(context =>
                    HasScope(context.User, "write:profile")));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        private static void AddGoogleProvider(
            Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            var googleConfig = configuration.GetSection("Authentication:Google");
            var clientId = googleConfig["ClientId"];
            var clientSecret = googleConfig["ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return;
            }

            authenticationBuilder.AddGoogle(options =>
            {
                options.SignInScheme = "Cookies";
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SaveTokens = true;
            });
        }

        private static void AddGitHubProvider(
            Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            var githubConfig = configuration.GetSection("Authentication:GitHub");
            var clientId = githubConfig["ClientId"];
            var clientSecret = githubConfig["ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return;
            }

            authenticationBuilder.AddOAuth("GitHub", options =>
            {
                options.SignInScheme = "Cookies";
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = githubConfig["CallbackPath"] ?? "/signin-github-callback";
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.SaveTokens = true;

                options.Scope.Add("read:user");
                options.Scope.Add("user:email");

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                options.ClaimActions.MapJsonKey("urn:github:name", "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        using var userRequest = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        userRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        userRequest.Headers.UserAgent.ParseAdd("Fabu");

                        using var userResponse = await context.Backchannel.SendAsync(
                            userRequest,
                            HttpCompletionOption.ResponseHeadersRead,
                            context.HttpContext.RequestAborted);
                        userResponse.EnsureSuccessStatusCode();

                        using var userDocument = JsonDocument.Parse(await userResponse.Content.ReadAsStringAsync(context.HttpContext.RequestAborted));
                        context.RunClaimActions(userDocument.RootElement);

                        if (context.Identity?.HasClaim(claim => claim.Type == ClaimTypes.Email) != true)
                        {
                            var email = await GetPrimaryGitHubEmailAsync(context);
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email));
                            }
                        }
                    }
                };
            });
        }

        private static async Task<string?> GetPrimaryGitHubEmailAsync(OAuthCreatingTicketContext context)
        {
            using var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
            emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            emailRequest.Headers.UserAgent.ParseAdd("Fabu");

            using var emailResponse = await context.Backchannel.SendAsync(
                emailRequest,
                HttpCompletionOption.ResponseHeadersRead,
                context.HttpContext.RequestAborted);
            if (!emailResponse.IsSuccessStatusCode)
            {
                return null;
            }

            using var emailDocument = JsonDocument.Parse(await emailResponse.Content.ReadAsStringAsync(context.HttpContext.RequestAborted));
            foreach (var item in emailDocument.RootElement.EnumerateArray())
            {
                var isPrimary = item.TryGetProperty("primary", out var primaryElement) && primaryElement.GetBoolean();
                var isVerified = item.TryGetProperty("verified", out var verifiedElement) && verifiedElement.GetBoolean();
                if (isPrimary && isVerified && item.TryGetProperty("email", out var emailElement))
                {
                    return emailElement.GetString();
                }
            }

            return null;
        }

        private static void AddOidcProvider(
            Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            var oidcConfig = configuration.GetSection("Authentication:Oidc");
            if (!oidcConfig.GetValue<bool>("Enabled")) return;

            var authority = oidcConfig["Authority"];
            var clientId = oidcConfig["ClientId"];
            var clientSecret = oidcConfig["ClientSecret"];

            if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("OIDC is enabled but Authority or ClientId is missing.");
            }

            authenticationBuilder.AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = authority;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ResponseType = "code";
                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = oidcConfig.GetValue("RequireHttpsMetadata", true);
                options.CallbackPath = oidcConfig["CallbackPath"] ?? "/signin-oidc-callback";
                options.SignedOutCallbackPath = oidcConfig["SignedOutCallbackPath"] ?? "/signout-callback-oidc";

                options.Scope.Clear();
                var scopes = oidcConfig.GetSection("Scopes").Get<string[]>()
                    ?? new[] { "openid", "profile", "email" };
                foreach (var scope in scopes.Where(scope => !string.IsNullOrWhiteSpace(scope)).Distinct())
                {
                    options.Scope.Add(scope);
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = ClaimTypes.Role,
                    ValidateIssuer = true
                };
            });
        }

        private static bool HasRole(ClaimsPrincipal user, string role)
        {
            return user.IsInRole(role) ||
                   user.Claims.Any(claim =>
                       claim.Type == ClaimTypes.Role &&
                       string.Equals(claim.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasPermission(ClaimsPrincipal user, params string[] permissions)
        {
            var permissionSet = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return user.Claims.Any(claim =>
                claim.Type == "permission" && permissionSet.Contains(claim.Value));
        }

        private static bool HasScope(ClaimsPrincipal user, string scope)
        {
            return user.Claims
                .Where(claim => claim.Type == "scope")
                .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Any(value => string.Equals(value, scope, StringComparison.OrdinalIgnoreCase));
        }
    }
}
