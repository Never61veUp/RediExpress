var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume(isReadOnly: false)
    .AddDatabase("RediExpressDb");

var migrations = builder.AddProject<Projects.RediExpress_PostgreSql_MigrationService>("migrations")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Projects.RediExpress_Host>("host")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WaitFor(migrations);

builder.Build().Run();