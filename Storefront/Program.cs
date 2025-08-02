using Contracts.Requests;
using Contracts.Responses;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using ServiceDefaults;
using Storefront;
using Storefront.Extensions.Mapping;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddGrpcClient<Messenger.MessengerClient>(options =>
	{
		string serverAddress = builder.Configuration.GetValue<string>("servicesUri")
			?? throw new InvalidOperationException("servicesUri is missing");

		options.Address = new Uri(serverAddress);
	});

builder.Services.AddLogging();
builder.Services.AddSingleton<CancellationTokenSource>();
builder.AddServiceDefaults();
WebApplication app = builder.Build();

app.MapPost("/w", WriteMessage);
app.MapGet("/g/{id}", GetUserMessages);

app.MapDefaultEndpoints();
app.Run();

static async Task<IResult> WriteMessage(
	[FromServices] Messenger.MessengerClient client,
	[FromBody] WriteMessageRestRequest request)
{
	WriteMessageRequest messageRequest = request.MapToGrpcRequest();
	WriteMessageResponse? response = await client.WriteMessageAsync(messageRequest);

	if (response == null || response.IsSuccessful is false)
	{
		return Results.NotFound();
	}

	return Results.Ok();
}

static async Task<IResult> GetUserMessages(
	int id,
	[FromServices] CancellationTokenSource src,
	[FromServices] ILogger<Program> logger,
	[FromServices] Messenger.MessengerClient client)
{
	List<GetUserMessageRestResponse> responses = new();

	using AsyncServerStreamingCall<GetMessageResponse>? streamResponse = client.GetUserMessages(new GetMessageRequest
	{
		RequestUserId = id
	});

	logger.LogInformation("Start of stream");
	src.Token.ThrowIfCancellationRequested();

	while (await streamResponse.ResponseStream.MoveNext(src.Token))
	{
		logger.LogInformation("entered the stream");
		GetMessageResponse? currentMessage = streamResponse.ResponseStream.Current;
		GetUserMessageRestResponse readyResponse = currentMessage.MapToRestResponse();
		logger.LogInformation($"message: {readyResponse.TextMessage}");

		responses.Add(readyResponse);
	}

	return Results.Ok(responses);
}