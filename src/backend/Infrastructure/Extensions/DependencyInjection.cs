using Application;
using Application.Common.Behaviors;
using Application.Interfaces;
using Application.Mapper;
using Application.Services;
using Application.Services.AIChatbot;
using Application.Services.Recommendations;
using Application.Services.Search;
using Application.Validators.LoginValidator;
using Application.Validators.UserValidator;
using FluentValidation;
using Infrastructure.Services;
using Infrastructure.Services.Search;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(IApplicationAssemblyMarker).Assembly;

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
            services.AddScoped<IRechargePlanRecommendationService, RechargePlanRecommendationService>();
            services.AddSingleton<IRechargePlanRecommendationEngine, RuleBasedRechargePlanRecommendationEngine>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IGlobalSearchService, GlobalSearchService>();
            services.AddSingleton<ISearchDocumentMapper, SearchDocumentMapper>();
            services.AddHttpClient<ElasticsearchSearchIndexService>();
            services.AddScoped<ISearchIndexService>(sp => sp.GetRequiredService<ElasticsearchSearchIndexService>());
            services.AddScoped<ElasticsearchSaveChangesInterceptor>();
            services.AddScoped<ICustomerSupportChatbotService, CustomerSupportChatbotService>();
            services.AddScoped<ICustomerSupportRagService, CustomerSupportRagService>();
            services.AddScoped<IChatbotConversationStore, ChatbotConversationStore>();
            services.AddSingleton<ICustomerSupportPromptBuilder, CustomerSupportPromptBuilder>();
            services.AddHttpClient<ConfigurableAiChatCompletionClient>();
            services.AddScoped<IAiChatCompletionClient>(sp => sp.GetRequiredService<ConfigurableAiChatCompletionClient>());
            services.AddScoped<IPaymentTransactionSagaService, PaymentTransactionSagaService>();
            services.AddScoped<IPaymentGateway, VNPayService>();
            services.AddHttpClient<PayPalService>();
            services.AddScoped<IPaymentGateway>(sp => sp.GetRequiredService<PayPalService>());
            services.AddHttpClient<StripeService>();
            services.AddScoped<IPaymentGateway>(sp => sp.GetRequiredService<StripeService>());
            services.AddHttpClient<ISmsService, SmsService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddValidatorsFromAssembly(assembly);
            services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            services.AddAutoMapper(assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            return services;
        }
    }
}
