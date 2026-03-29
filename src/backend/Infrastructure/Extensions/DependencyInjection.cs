//using Application.Interfaces;
//using Application.Validators.LoginValidator;
//using Application.Validators.UserValidator;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using FluentValidation;
//using Microsoft.Extensions.DependencyInjection;
//using Application.Services;
//using Infrastructure.Services;
//using Microsoft.Identity.Client;

//namespace Infrastructure.Extensions
//{
//    public static class DependencyInjection
//    {
//        public static IServiceCollection AddApplication(this IServiceCollection services)
//        {
//            // Lấy Assembly của chính project Application
//            var assembly = typeof(DependencyInjection).Assembly;

//            // Register services
//            services.AddScoped<IResponseCacheService, ResponseCacheService>();
//            services.AddScoped<IUserService, UserService>();
//            services.AddScoped<IAuthService, AuthService>();
//            services.AddScoped<IRoleService, RoleService>();
//            services.AddScoped<IPermissionService, PermissionService>();
//            services.AddScoped<ITransactionService, TransactionService>();
//            services.AddScoped<IPaymentService, PaymentService>();
//            services.AddScoped<IAccountService, AccountService>();
//            services.AddScoped<IAuditLogService, AuditLogService>();
//            services.AddScoped<ICouponService, CouponService>();
//            services.AddScoped<ICouponUsageService, CouponUsageService>();
//            services.AddScoped<ICustomerService, Application.Services.CustomerService>();
//            services.AddScoped<ICustomerServiceService, CustomerServiceService>();
//            services.AddScoped<IFeedbackService, FeedbackService>();
//            services.AddScoped<IPostpaidBillService, PostpaidBillService>();
//            services.AddScoped<IRechargePlanService, RechargePlanService>();
//            services.AddScoped<IServiceService, ServiceService>();

//            // Đăng ký tất cả Validators từ Assembly này
//            services.AddValidatorsFromAssembly(assembly);
//            services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
//            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

//            // Đăng ký AutoMapper từ Assembly này
//            services.AddAutoMapper(assembly);

//            return services;
//        }
//    }
//}

using Application.Interfaces;
using Application.Validators.LoginValidator;
using Application.Validators.UserValidator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Infrastructure.Services;
using Microsoft.Identity.Client;
using Domain.Repositories;
using Persistence.Repositories;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Lấy Assembly của chính project Application
            var assembly = typeof(DependencyInjection).Assembly;

            // Register services
            services.AddScoped<ICustomerServiceRepository, CustomerServiceRepository>();
            services.AddScoped<ICustomerServicesService, CustomerServicesService>();
            services.AddScoped<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICouponUsageService, CouponUsageService>();
            services.AddScoped<ICustomersService, Application.Services.CustomersService>();
            services.AddScoped<ICustomerServicesService, CustomerServicesService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IPostpaidBillService, PostpaidBillService>();
            services.AddScoped<IRechargePlanService, RechargePlanService>();
            services.AddScoped<IServiceService, ServiceService>();

            // Đăng ký tất cả Validators từ Assembly này
            services.AddValidatorsFromAssembly(assembly);
            services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            // Đăng ký AutoMapper từ Assembly này
            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
