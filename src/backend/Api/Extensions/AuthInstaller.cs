using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data.Contexts;
using Persistence.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Extensions
{
    public class AuthInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var authConfig = configuration.GetSection("Auth");
            if (!authConfig.GetValue<bool>("Enabled")) return;

            //identity
            services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();
            
            //JWT

            var jwtSettings = configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured in the application settings.");
            }
            var key = Encoding.UTF8.GetBytes(jwtKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
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
                    //event de populate claims tu db sau validate - userId, role
                    if (authConfig.GetValue<bool>("CheckBanEnabled"))
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = async context =>
                            {
                                var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                                if (userId == null)
                                {
                                    context.Fail("Token invalid");
                                    return;
                                }

                                var userManager = context.HttpContext.RequestServices
                                    .GetRequiredService<UserManager<ApplicationUser>>();

                                var user = await userManager.FindByIdAsync(userId);

                                if (user == null)
                                {
                                    context.Fail("User not found");
                                    return;
                                }

                                if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
                                {
                                    context.Fail("User banned");
                                }
                            }
                        };
                    }
                });
              //ROLE_ADMIN
              //├─ user.create
              //├─ user.delete
              //├─ order.refund

            //authorization 
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserDelete", policy => policy.RequireClaim("permission", "user.delete"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireScope", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("write:profile")))); 
            });

            //cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); //de nhan/gui cookie
                });
            });
        }
    }
}
