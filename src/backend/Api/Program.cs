using Api.Extensions;
using Api.Extensions.ContextExtensions;
using Api.Middleware;
using Application.Extensions;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Configurations;
using Hangfire;
using Infrastructure.Services;
using Persistence.Data.Configurations;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

//DI tu dong redis, mail, payment
builder.Services.InstallerServicesInAssembly(builder.Configuration);
builder.Services.AddHttpContextAccessor(); //httpContextAccessor cho claims
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

//hien them
builder.Services.AddScoped<IResponseCacheService, MemoryResponseCacheService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//Queue + Retry Email
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();
app.UseMiddleware<GlobalException>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); //chi tiet loi chi dev - misconfiguration fix
}
else
{
    app.UseHsts(); // https strict transport - misconfiguration fix
    app.UseExceptionHandler("/Error"); //generic error o prod
}
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseRateLimiter();
app.UseHangfireDashboard(); //hien them
app.MapReverseProxy();
app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
