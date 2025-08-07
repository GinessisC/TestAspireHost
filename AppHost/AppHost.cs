using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var postgres =
	builder.AddPostgres("postgres")
		.WithDataVolume()
		.WithPgAdmin();

var db = postgres.AddDatabase("postgres-db");
var dataProvider = builder.AddProject<DataProvider>("DataProvider")
	.WithReference(db)
	.WaitFor(db);

var services = builder.AddProject<Services>("Services")
	.WithReference(dataProvider);

builder.AddProject<Storefront>("frontend")
	.WithReference(services);

DistributedApplication app = builder.Build();

app.Run();