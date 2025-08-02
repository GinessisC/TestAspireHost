using DataProvider;
using Grpc.Core;
using Services.Entities;
using Storefront;

namespace Services.Services;

public class MessengerService : Messenger.MessengerBase
{
	private readonly MessengerClientService _messengerClientService;

	public MessengerService(MessengerClientService messengerClientService)
	{
		_messengerClientService = messengerClientService;
	}

	public override async Task<WriteMessageResponse> WriteMessage(WriteMessageRequest request,
		ServerCallContext context)
	{
		MessageEntity message = new()
		{
			IssuerId = request.RequestUserId,
			ReceiverId = request.ResponseUserId,
			TextMessage = request.TextMessage,
			MessageId = Guid.NewGuid().ToString()
		};

		WriteMessageResultModel responseFromDb = await _messengerClientService.WriteMessageAsync(message);

		return new WriteMessageResponse
		{
			IsSuccessful = responseFromDb.IsSuccessful,
			ErrorDescription = responseFromDb.ErrorDescription
		};
	}

	public override async Task GetUserMessages(GetMessageRequest request,
		IServerStreamWriter<GetMessageResponse> responseStream,
		ServerCallContext context)
	{
		IAsyncEnumerable<MessageFromDb> messages =
			await _messengerClientService.GetUserMessagesByUserIdAsync(request.RequestUserId);

		await foreach (MessageFromDb dbMessage in messages)
		{
			//TODO: get mapping work out
			GetMessageResponse response = new GetMessageResponse
			{
				Message = dbMessage.Message,
				MessageId = dbMessage.MessageId,
				ReceiverId = dbMessage.UserId
			};

			await responseStream.WriteAsync(response);
		}
	}
}