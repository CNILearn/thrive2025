var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres")
    .WithPgWeb()
    .WithDataVolume("postgres-books-volume")
    .AddDatabase("postgres-books-db");


var api = builder.AddProject<Projects.ExistingAPI>("existingapi")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
