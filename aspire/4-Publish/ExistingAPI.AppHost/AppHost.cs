using Aspire.Hosting.Azure;

#pragma warning disable ASPIREAZURE001 // in preview
#pragma warning disable ASPIRECOMPUTE001 // in preview

var builder = DistributedApplication.CreateBuilder(args);

// az deployment group create --template-file container-apps.bicep

// builder.AddAzureContainerAppEnvironment("container-apps");
// builder.AddAzureAppServiceEnvironment("appservice");
// builder.AddDockerComposeEnvironment("docker-compose");
builder.AddKubernetesEnvironment("k8s");


var db = builder.AddPostgres("postgres")
            .WithPgWeb()
            .WithDataVolume("postgres-books-volume")
            .AddDatabase("postgres-books-db");

var api = builder.AddProject<Projects.ExistingAPI>("existingapi")
    .WithReference(db)
    .WaitFor(db)
    .WithExternalHttpEndpoints();

builder.Build().Run();
