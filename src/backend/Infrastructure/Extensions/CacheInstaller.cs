using Application.Interfaces;
using Domain.Data.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Extensions
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>()
                ?? new RedisConfiguration { Enabled = true };
            services.AddSingleton(redisConfig);
            // Luôn register IDistributedCache
            if (redisConfig.Enabled && !string.IsNullOrEmpty(redisConfig.ConnectionStrings))
            {
                // Redis thật
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    try
                    {
                        return ConnectionMultiplexer.Connect(redisConfig.ConnectionStrings);
                        Console.WriteLine("Redis oke");
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng vẫn fallback
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Using in-memory cache.");
                        return null; // hoặc throw nếu muốn bắt buộc Redis
                    }
                });
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfig.ConnectionStrings;
                    options.InstanceName = "Fabu:";
                });
            }
            else
            {
                // Fallback in-memory (rất quan trọng!)
                services.AddDistributedMemoryCache();
            }
            // Register service scoped
            services.AddScoped<IResponseCacheService, ResponseCacheService>();
        }
    }
}
