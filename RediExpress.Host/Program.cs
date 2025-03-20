using RediExpress.Application.Services;
using RediExpress.Auth.Abstractions;
using RediExpress.Auth.Model;
using RediExpress.Auth.Services;
using RediExpress.Host.Extensions;
using RediExpress.PostgreSql;
using RediExpress.PostgreSql.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

builder.Services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.AddNpgsqlDbContext<RediExpressDbContext>("RediExpressDb", options =>
{
    options.DisableHealthChecks = true;
    options.DisableTracing = true;
});

var opt = configuration.GetSection(nameof(JwtOptions));
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