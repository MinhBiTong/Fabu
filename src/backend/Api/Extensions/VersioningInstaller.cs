using Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Api.Extensions
{
    public class VersioningInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // /api/v1/auth/login
            });
        }
    }
}
