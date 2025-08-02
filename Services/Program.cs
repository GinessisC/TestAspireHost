using DataProvider;
using ServiceDefaults;
using Services.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services
	.AddGrpcClient<MessengerRepository.MessengerRepositoryClient>(options =>
	{
		string dataProviderUri = builder.Configuration.GetValue<string>("DataProviderUri")
			?? throw new NullReferenceException("No uri for dataProvider was specified");

		options.Address = new Uri(dataProviderUri);
	});

builder.Services.AddLogging();

builder.Services.AddScoped<MessengerClientService>();
builder.AddServiceDefaults(); //from service defaults for aspire

WebApplication app = builder.Build();
app.MapGrpcService<MessengerService>();

app.MapGet("/", () => "Hello World!");

app.MapDefaultEndpoints(); //from service defaults for aspire
app.Run();