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

            var connectionString = redisConfig.EffectiveConnectionString;
            if (redisConfig.Enabled && !string.IsNullOrWhiteSpace(connectionString))
            {
                try
                {
                    var multiplexer = ConnectionMultiplexer.Connect(connectionString);
                    services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = connectionString;
                        options.InstanceName = string.Empty;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Redis connection failed: {ex.Message}. Using in-memory cache.");
                    services.AddDistributedMemoryCache();
                }
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddScoped<IResponseCacheService, ResponseCacheService>();
        }
    }
}
