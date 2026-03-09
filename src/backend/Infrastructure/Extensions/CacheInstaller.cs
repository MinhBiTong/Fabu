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
            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);  // Bind config tu appseting OK 
            services.AddSingleton(redisConfiguration);

            if (!redisConfiguration.Enabled) return;  //Nếu disabled thì skip, ngược lại add service

            var connectionString = redisConfiguration.ConnectionStrings;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Redis connection string is empty.");

            services.AddSingleton<IConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(connectionString));  // inject add IConnectionMultiplexer singleton neu enable

            services.AddStackExchangeRedisCache(option => {
                option.Configuration = redisConfiguration.ConnectionStrings;
                option.InstanceName = "Fabu";
            });


            //quan ly viec cache 1 cai la interface, 1 cai la implement
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
