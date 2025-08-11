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
builder.AddServiceDefaults();
WebApplication app = builder.Build();

app.MapPost("/write", WriteMessage);
app.MapGet("/get/{id}", GetUserMessages);

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
		return TypedResults.NotFound();
	}
	return TypedResults.Ok();
}

static async Task<IResult> GetUserMessages(
	int id,
	CancellationToken ct,
	[FromServices] Messenger.MessengerClient client)
{
	List<GetUserMessageRestResponse> responses = new();

	using AsyncServerStreamingCall<GetMessageResponse>? streamResponse = client.GetUserMessages(new GetMessageRequest
	{
		RequestUserId = id
	});
	
	while (await streamResponse.ResponseStream.MoveNext(ct))
	{
		GetMessageResponse? currentMessage = streamResponse.ResponseStream.Current;
		GetUserMessageRestResponse readyResponse = currentMessage.MapToRestResponse();

		responses.Add(readyResponse);
	}
	return TypedResults.Ok(responses);
}