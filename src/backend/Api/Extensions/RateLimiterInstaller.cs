using Application.Interfaces;
using Domain.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.RateLimiting;


namespace Api.Extensions
{
    //rate limiting de chong brute force/spam
    public class RateLimiterInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //1, BIND config tu appsetting
            services.Configure<RateLimiterConfiguration>(configuration.GetSection("RateLitting"));

            //lay gia tri config ngay luc khoi tao de toi uu performance
            var config = configuration.GetSection("RateLimiting").Get<RateLimiterConfiguration>()
                ?? new RateLimiterConfiguration();

            services.AddRateLimiter(options =>
            {
                //2, cau hinh phan hoi khi bi chan
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        Status = 429,
                        Message = "The system is busy due to a high volume of requests. Please try again in a few minutes."
                    }, cancellationToken: token);
                };

                //3, global limiter: bao ve toan bo he thong - AntiforgeryApplicationBuilderExtensions-DDOS
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "global",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20, // 20 req cho toan site
                            Window = TimeSpan.FromSeconds(1),
                            AutoReplenishment = true
                        });
                });

                //4, policy danh cho auth - login/register - chong brute force
                //chan base on ip address + endpoint path
                options.AddPolicy("AuthPolicy", httpContext =>
                {
                    // Lấy IP của người dùng làm khóa định danh
                    // Nếu dùng Proxy/Load Balancer, hãy đảm bảo đã cấu hình Forwarded Headers
                    var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    // Kết hợp IP với đường dẫn API để tránh việc chặn nhầm các API khác của cùng 1 IP
                    var partitionKey = $"auth_{remoteIp}_{httpContext.Request.Path}";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = config.PermitLimit,
                        Window = TimeSpan.FromSeconds(config.WindowInSeconds),
                        QueueLimit = config.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
                });
            });
        }
    }
}
