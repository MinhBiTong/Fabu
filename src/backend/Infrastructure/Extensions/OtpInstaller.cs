using Application.Interfaces;
using Domain.Options;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public class OtpInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmsConfiguration>(configuration.GetSection("Sms"));
            services.AddHttpClient<ISmsService, SmsService>();
        }
    }
}
