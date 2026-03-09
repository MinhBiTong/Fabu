using Application.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Domain.Configurations;


namespace Api.Extensions
{
    //rate limiting de chong brute force/spam
    public class RateLimiterInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RateLimiterConfiguration>()
                .Bind(configuration.GetSection("RateLimiting"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    {
                        var serviceProvider = httpContext.RequestServices;
                        var config = serviceProvider
                            .GetRequiredService<IOptions<RateLimiterConfiguration>>()
                            .Value;

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey:
                                httpContext.User.Identity?.Name
                                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                ?? "anonymous",

                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = config.PermitLimit,
                                Window = TimeSpan.FromSeconds(config.WindowInSeconds),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = config.QueueLimit,
                                AutoReplenishment = true
                            });
                    });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }
    }
}
