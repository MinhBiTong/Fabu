using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Api.Extensions
{
    public class SystemInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; //giu nguyen ten thuoc tinh khi serialize
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //fix cycle
                options.JsonSerializerOptions.MaxDepth = 64;
                options.JsonSerializerOptions.WriteIndented = true;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Store API",
                    Version = "v1"
                });
            });
        }
    }
}
