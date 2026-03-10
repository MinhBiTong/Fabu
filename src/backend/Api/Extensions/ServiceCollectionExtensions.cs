using Application.Interfaces;
using Application.Validators.LoginValidator;
using Application.Validators.UserValidator;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data.Contexts;
using Persistence.Repositories;
using System.Reflection;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //Add DbContext, UnitOfWork, Repositories.
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            return services;
        }

        //Add Services (IUserService), AutoMapper, FluentValidation.
        //public static IServiceCollection AddApplication(this IServiceCollection services)
        //{
        //    services.AddScoped<IUserService, UserService>();
        //    services.AddScoped<IAuthService, AuthService>();
        //    services.AddScoped<IRoleService, RoleService>();
        //    services.AddScoped<IPermissionService, PermissionService>();
        //    // Lấy Assembly của chính project Application
        //    var assembly = typeof(CreateUserRequestValidator).Assembly;

        //    // Đăng ký tất cả Validators từ Assembly này
        //    services.AddValidatorsFromAssembly(assembly);
        //    services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        //    services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        //    //services.AddAutoMapper(typeof(MappingProfile));

        //    // Đăng ký AutoMapper từ Assembly này
        //    services.AddAutoMapper(assembly);

        //    return services;
        //}
       
        //scan assembly, tim tat ca IInstaller classes nhu CacheInstaller, SystemInstaller
        //tao instance + goi InstallService cho tung cai
        public static void InstallerServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            //lay het tat ca class trong Installer va bo di interface va abstract class 
            var currentAssesmbly = Assembly.GetExecutingAssembly();
            var installers = currentAssesmbly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList();

            //check
            if (installers == null || !installers.Any()) return;

            installers.ForEach(i => i.InstallServices(services, configuration));
        }
    }
}
