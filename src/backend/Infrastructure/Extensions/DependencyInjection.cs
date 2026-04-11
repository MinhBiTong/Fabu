using Application.Interfaces;
using Application.Mapper;
using Application.Services;
using Application.Validators.LoginValidator;
using Application.Validators.UserValidator;
using AutoMapper;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Lấy Assembly của chính project Application
            var assembly = typeof(DependencyInjection).Assembly;

            // Register services
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
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerServicesService, CustomerServicesService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IPostpaidBillService, PostpaidBillService>();
            services.AddScoped<IRechargePlanService, RechargePlanService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPaymentGateway, VNPayService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IEmailService, EmailService>();
            // Đăng ký tất cả Validators từ Assembly này
            services.AddValidatorsFromAssembly(assembly);
            services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            // Đăng ký AutoMapper từ Assembly này
            services.AddAutoMapper(assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}

