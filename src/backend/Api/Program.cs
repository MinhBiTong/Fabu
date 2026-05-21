using Api.Extensions;
using Api.Extensions.ContextExtensions;
using Api.Middleware;
using Application.Interfaces;
using Application.Services;
using Domain.Abstractions;
using Domain.Configurations;
using Domain.Entities;
using Domain.Options;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data.Configurations;
using Persistence.Data.Contexts;
using Persistence.Repositories;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//DI tu dong redis, mail, payment
builder.Services.InstallerServicesInAssembly(builder.Configuration);
builder.Services.AddHttpContextAccessor(); //httpContextAccessor cho claims
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); //cau hinh reverse proxy tu appsettings.json
builder.Services.Configure<UserConfiguration>(builder.Configuration.GetSection("UserSettings"));
builder.Services.Configure<RoleConfiguration>(builder.Configuration.GetSection("RoleSettings"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<PermissionConfiguration>(builder.Configuration.GetSection("PermissionSettings"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RateLimiterConfiguration>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<AIChatbotConfiguration>(builder.Configuration.GetSection("AIChatbot"));
builder.Services.Configure<SmsConfiguration>(builder.Configuration.GetSection("Sms"));
builder.Services.AddScoped<Domain.Abstractions.IUserContext, UserContext>();
//builder.Services.Configure<VNPayConfiguration>(builder.Configuration.GetSection("VNPay"));
builder.Services.AddOptions<VNPayConfiguration>()
        .Bind(builder.Configuration.GetSection("VNPay"))
        .ValidateDataAnnotations()
        .ValidateOnStart();
builder.Services.AddOptions<PayPalConfiguration>()
        .Bind(builder.Configuration.GetSection("PayPal"))
        .ValidateDataAnnotations()
        .ValidateOnStart();
builder.Services.AddOptions<StripeConfiguration>()
        .Bind(builder.Configuration.GetSection("Stripe"))
        .ValidateDataAnnotations()
        .ValidateOnStart();

//builder.Logging.ClearProviders(); 
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext());

//Queue + Retry Email
//builder.Services.AddHangfireServer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
var app = builder.Build();
app.UseMiddleware<GlobalException>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fabu v1");
        c.RoutePrefix = "swagger";  // mặc định là swagger
    });
    app.UseDeveloperExceptionPage(); //chi tiet loi chi dev - misconfiguration fix
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.UseSerilogRequestLogging();
app.MapReverseProxy();

app.Run();

