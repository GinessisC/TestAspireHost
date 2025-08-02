using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Storefront>("frontend");

DistributedApplication app = builder.Build();

app.Run();