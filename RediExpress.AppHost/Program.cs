var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(configureContainer: containerBuilder =>
    {
        containerBuilder.WithHostPort(60037);
    })
    .WithDataVolume(isReadOnly: false)
    .AddDatabase("RediExpressDb");


var migrations = builder.AddProject<Projects.RediExpress_PostgreSql_MigrationService>("migrations")
    .WithReference(postgres)
    .WaitFor(postgres);

var cache = builder.AddRedis("cache")
    .WithRedisInsight()
    .WithDataVolume(isReadOnly: false);

var ordersRedis = builder.AddRedis("orders")
    .WithDataVolume();

var passwordResetRedis = builder.AddRedis("password-reset")
    .WithDataVolume();

var chatRedis = builder.AddRedis("chat")
    .WithDataVolume();


builder.AddProject<Projects.RediExpress_Host>("host")
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(ordersRedis)
    .WithReference(passwordResetRedis)
    .WithReference(chatRedis)
    .WaitFor(postgres)
    .WaitFor(cache)
    .WaitFor(migrations)
    .WaitFor(ordersRedis)
    .WaitFor(passwordResetRedis)
    .WaitFor(chatRedis);

builder.Build().Run();