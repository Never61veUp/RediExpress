using RediExpress.PostgreSql;
using RediExpress.PostgreSql.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));


builder.AddNpgsqlDbContext<RediExpressDbContext>("RediExpressDb", options =>
{
    options.DisableTracing = true;
    
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();