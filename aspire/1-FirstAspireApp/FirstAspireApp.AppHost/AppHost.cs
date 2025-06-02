var builder = DistributedApplication.CreateBuilder(args);

var sqlDb = builder.AddSqlServer("SqlServer")
    .WithDataVolume("sqlserver-data")
    .AddDatabase("mysqldb");

var apiService = builder.AddProject<Projects.FirstAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(sqlDb)
    .WaitFor(sqlDb);

builder.AddProject<Projects.FirstAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
