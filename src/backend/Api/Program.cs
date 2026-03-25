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
