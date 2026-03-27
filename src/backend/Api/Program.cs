using Microsoft.Extensions.Caching.Memory;
using Api.Extensions;
using Api.Extensions.ContextExtensions;
using Api.Middleware;
using Infrastructure.Extensions;
using Application.Interfaces;
using Application.Services;
using Domain.Abstractions;
using Domain.Configurations;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data.Configurations;
using Persistence.Data.Contexts;
using Persistence.Repositories;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//DI tu dong redis, mail, payment
builder.Services.InstallerServicesInAssembly(builder.Configuration);
builder.Services.AddScoped<IResponseCacheService, ResponseCacheService>();
builder.Services.AddHttpContextAccessor(); //httpContextAccessor cho claims
builder.Services.AddDistributedMemoryCache();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); //cau hinh reverse proxy tu appsettings.json
builder.Services.Configure<UserConfiguration>(builder.Configuration.GetSection("UserSettings"));
builder.Services.Configure<RoleConfiguration>(builder.Configuration.GetSection("RoleSettings"));
builder.Services.Configure<PermissionConfiguration>(builder.Configuration.GetSection("PermissionSettings"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RateLimiterConfiguration>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IUserContext, UserContext>();
//builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
// Đăng ký bộ nhớ tạm mặc định
builder.Services.AddMemoryCache();

// Đăng ký class giả để "lừa" hệ thống
//// ĐẢM BẢO DÒNG NÀY NẰM NGAY DƯỚI ĐOẠN VỪA DÁN
//var app = builder.Build(); 
//builder.Services.AddDistributedMemoryCache();

//// Bật Swagger NGAY SAU KHI Build
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "greenginger v1");
//    c.RoutePrefix = string.Empty;
//});

//// Sau đó mới đến đoạn IF check môi trường
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}
//// CHỈ chi tiết lỗi khi ở môi trường Dev
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

//// QUAN TRỌNG: Tắt dòng này vì bạn đang chạy cổng http (5000)
//// app.UseHttpsRedirection(); 

//app.UseStaticFiles();
//app.UseRouting();
//// ... các dòng app.UseCors, Authentication, Authorization giữ nguyên
////if (app.Environment.IsDevelopment())
////{
////    app.UseDeveloperExceptionPage(); //chi tiet loi chi dev - misconfiguration fix
////}
////else
////{
////    app.UseHsts(); // https strict transport - misconfiguration fix
////    app.UseExceptionHandler("/Error"); //generic error o prod
////}

//////app.UseHttpsRedirection();
//app.UseRateLimiter();
////app.MapReverseProxy();

//app.UseAuthentication();
////app.UseMiddleware<TokenBlacklistMiddleware>();
//app.UseAuthorization();
//app.UseCors("AllowReactApp");
//app.MapControllers();

//app.Run();
// Đăng ký Cache TRƯỚC KHI Build
builder.Services.AddDistributedMemoryCache();

// Đăng ký Google Auth và đọc từ appsettings.json
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
//                           ?? throw new InvalidOperationException("Missing Google ClientId");
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
//                               ?? throw new InvalidOperationException("Missing Google ClientSecret");
//    });

// GỌI BUILD ĐÚNG 1 LẦN DUY NHẤT SAU KHI ĐÃ ADD HẾT SERVICES

// --- BẮT ĐẦU ĐOẠN ÉP BUỘC XÓA REDIS ---
var oldCacheService = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IResponseCacheService));
if (oldCacheService != null)
{
    builder.Services.Remove(oldCacheService); // Nhẫn tâm gỡ bỏ dịch vụ cũ đang bị lỗi
}
builder.Services.AddMemoryCache();
// --- KẾT THÚC ĐOẠN ÉP BUỘC ---

//builder.Logging.ClearProviders(); 
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext());

var app = builder.Build();
app.UseMiddleware<GlobalException>();

// Bật Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "greenginger v1");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "greenginger v1");
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
app.UseMiddleware<GlobalException>();
app.UseAuthorization();
app.MapControllers();
app.UseSerilogRequestLogging();
app.MapReverseProxy();

app.Run();
