using Application.Interfaces;
using Application.Services;
using Application.Validators.LoginValidator;
using Application.Validators.UserValidator;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Lấy Assembly của chính project Application
            var assembly = typeof(DependencyInjection).Assembly;

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRechargePlanService, RechargePlanService>();

            // Đăng ký tất cả Validators từ Assembly này
            services.AddValidatorsFromAssembly(assembly);
            //services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
            //services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            // Đăng ký AutoMapper từ Assembly này
            services.AddAutoMapper(cfg => {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
