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
                try 
                {
                    services.AddSingleton<IConnectionMultiplexer>(sp =>
                        ConnectionMultiplexer.Connect(redisConfig.ConnectionStrings));

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConfig.ConnectionStrings;
                        options.InstanceName = "Fabu:";
                    });

                    services.AddScoped<IResponseCacheService, ResponseCacheService>();
                }
                catch
                {
                    // fallback nếu Redis lỗi
                    services.AddDistributedMemoryCache();
                    services.AddScoped<IResponseCacheService, MemoryResponseCacheService>();
                }
            }
            else
            {
                // Fallback: In-memory cache khi Redis disabled (cho dev/test)
                services.AddDistributedMemoryCache();
                //hien thêm
                Console.WriteLine("Using Memory Cache");
                services.AddScoped<IResponseCacheService, MemoryResponseCacheService>();
                Console.WriteLine("Registered MemoryResponseCacheService");
                return;
            }

            //Register service(luôn có, dù Redis hay không)
            //quan ly viec cache 1 cai la interface, 1 cai la implement
            //hien comment
            //services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            //services.AddScoped<IResponseCacheService, ResponseCacheService>(); // Đăng ký cả Singleton và Scoped để đảm bảo có instance khi Redis disabled

            //services.AddScoped<IResponseCacheService, MemoryResponseCacheService>();

        }
    }
}
