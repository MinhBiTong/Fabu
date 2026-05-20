using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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

                if (authConfig.GetValue<bool>("CheckBanEnabled"))
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (userId is null)
                            {
                                throw new AppException(ErrorCode.UNAUTHENTICATED);
                            }

                            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                            var user = await userManager.FindByIdAsync(userId);
                            if (user is null)
                            {
                                throw new AppException(ErrorCode.UNAUTHENTICATED);
                            }
                        }
                    };
                }
            });

            AddGoogleProvider(authenticationBuilder, configuration);
            AddOidcProvider(authenticationBuilder, configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserDelete", policy => policy.RequireClaim("permission", "user.delete"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireScope", policy => policy.RequireAssertion(context =>
                    context.User.HasClaim(claim => claim.Type == "scope" && claim.Value.Contains("write:profile"))));
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
    }
}
