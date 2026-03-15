using Application.Interfaces;
using Domain.Data.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using StackExchange.Redis;

namespace Infrastructure.Extensions
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>()
                ?? new RedisConfiguration { Enabled = false };

            services.AddSingleton(redisConfig);

            if (redisConfig.Enabled && !string.IsNullOrEmpty(redisConfig.ConnectionStrings))
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConfig.ConnectionStrings));

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfig.ConnectionStrings;
                    options.InstanceName = "Fabu:";
                });
            }
            else
            {
                // Fallback: In-memory cache khi Redis disabled (cho dev/test)
                services.AddDistributedMemoryCache();
            }

            //Register service(luôn có, dù Redis hay không)
            //quan ly viec cache 1 cai la interface, 1 cai la implement
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IResponseCacheService, ResponseCacheService>(); // Đăng ký cả Singleton và Scoped để đảm bảo có instance khi Redis disabled
        }
    }
}
