using RediExpress.Application.Services;
using RediExpress.Auth.Abstractions;
using RediExpress.Auth.Model;
using RediExpress.Auth.Services;
using RediExpress.EmailService.Configuration;
using RediExpress.EmailService.Services;
using RediExpress.GeoService.Services;
using RediExpress.Host.Extensions;
using RediExpress.PostgreSql;
using RediExpress.PostgreSql.Repositories;
using RediExpress.Redis.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IGeoService, GeoService>();
builder.Services.AddHttpClient<HttpMessageInvoker, HttpClient>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRedisService, OrderRedisService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRiderService, RiderService>();
builder.Services.AddScoped<IRiderRepository, RiderRepository>();

builder.AddRedisClient(connectionName: "cache");
builder.AddKeyedRedisClient(name: "orders");
builder.AddKeyedRedisClient(name: "password-reset");
builder.AddKeyedRedisClient(name: "chat");

builder.AddNpgsqlDbContext<RediExpressDbContext>("RediExpressDb", options =>
{
    options.DisableHealthChecks = true;
    options.DisableTracing = true;
});

builder.Services.Configure<MailSettings>(
    builder
        .Configuration
        .GetSection(nameof(MailSettings))
);

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.AddApiAuthentication(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();