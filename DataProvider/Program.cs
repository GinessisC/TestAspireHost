using DataProvider;
using DataProvider.Abstractions.Repositories;
using DataProvider.Entities;
using DataProvider.Repositories;
using DataProvider.Services;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
	//TODO what is builder.Configuration.GetConnectionString - read about it
	optionsBuilder.UseNpgsql(builder.Configuration["DbConnectionString"]);
});

builder.Services.AddLogging();

builder.Services.AddScoped<IRepository<MessageEntity>, Repository<MessageEntity>>();
builder.Services.AddScoped<IRepository<UserEntity>, Repository<UserEntity>>();

builder.AddServiceDefaults(); //from service defaults for aspire
builder.Services.AddGrpc();

WebApplication app = builder.Build();
app.MapGrpcService<AppService>();

app.MapGet("/", () => "Hello World!");

app.MapDefaultEndpoints(); //from service defaults for aspire

app.Run();