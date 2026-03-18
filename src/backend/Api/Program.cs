using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Persistence.Data.Contexts;
using Domain.Abstractions;
using Persistence.Repositories;
using Api.Extensions;
using Api.Middleware;
using Application.Extensions;
using Persistence.Data.Configurations;
using Domain.Configurations;
using Api.Extensions.ContextExtensions;
using Application.Interfaces;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//DI tu dong redis, mail, payment
builder.Services.InstallerServicesInAssembly(builder.Configuration);
builder.Services.AddHttpContextAccessor(); //httpContextAccessor cho claims
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
//builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); //cau hinh reverse proxy tu appsettings.json
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
builder.Services.AddSingleton<IResponseCacheService, DummyCacheService>();

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
builder.Services.AddSingleton<IResponseCacheService, DummyCacheService>(); // Ép dùng dịch vụ giả của chúng ta
// --- KẾT THÚC ĐOẠN ÉP BUỘC ---

var app = builder.Build();

// Bật Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "greenginger v1");
    c.RoutePrefix = string.Empty;
});

// Check môi trường (Mình đã xóa đoạn bị lặp lại của bạn)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowReactApp");
app.MapControllers();

app.Run();

public class DummyCacheService : IResponseCacheService
{
    private readonly IMemoryCache _cache;
    public DummyCacheService(IMemoryCache cache) => _cache = cache;

    public Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
    {
        _cache.Set(cacheKey, response, timeOut);
        return Task.CompletedTask;
    }

    public Task<T?> GetCachedResponseAsync<T>(string cacheKey)
    {
        // Ép kiểu chuẩn C# để không bị lỗi 'out T'
        _cache.TryGetValue(cacheKey, out object? result);
        return Task.FromResult(result == null ? default : (T)result);
    }

    public Task RemoveCacheResponseAsync(string cacheKey)
    {
        _cache.Remove(cacheKey);
        return Task.CompletedTask;
    }
}