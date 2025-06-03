var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FirstAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

if (builder.ExecutionContext.IsPublishMode)
{
    // In publish mode, we use Azure SQL Server instead of local SQL Server
    var sqlDb = builder.AddAzureSqlServer("sqlServer")
        .AddDatabase("mysqldb");

    apiService.WithReference(sqlDb).WaitFor(sqlDb);
}
else
{
    var sqlDb = builder.AddSqlServer("sqlserver")
        .WithDataVolume("sqlserver-data")
        .AddDatabase("mysqldb");

    apiService.WithReference(sqlDb).WaitFor(sqlDb);
}

builder.AddProject<Projects.FirstAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
