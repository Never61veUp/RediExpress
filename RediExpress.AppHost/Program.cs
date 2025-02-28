var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume(isReadOnly: false)
    .AddDatabase("RediExpressDb");

builder.AddProject<Projects.RediExpress_Host>("host")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();