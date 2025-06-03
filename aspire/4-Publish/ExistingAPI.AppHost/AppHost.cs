using Aspire.Hosting.Azure;

#pragma warning disable ASPIREAZURE001 // Type or member is obsolete

var builder = DistributedApplication.CreateBuilder(args);

// builder.AddAzureAppServiceEnvironment("appservice");
builder.AddAzureEnvironment();
// az deployment sub create --location westeurope --template-file main.bicep

// builder.AddDockerComposeEnvironment("docker-compose");
//builder.AddKubernetesEnvironment("k8s");

var db = builder.AddPostgres("postgres")
    .WithPgWeb()
    .WithDataVolume("postgres-books-volume")
    .AddDatabase("postgres-books-db");

var api = builder.AddProject<Projects.ExistingAPI>("existingapi")
    .WithReference(db)
    .WaitFor(db)
    .WithExternalHttpEndpoints();

builder.Build().Run();
