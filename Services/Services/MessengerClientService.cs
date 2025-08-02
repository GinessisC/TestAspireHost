using DataProvider;
using Grpc.Core;
using Services.Entities;
using static DataProvider.MessengerRepository;

namespace Services.Services;

public class MessengerClientService
{
	private readonly MessengerRepositoryClient _repositoryClient;

	public MessengerClientService(MessengerRepositoryClient repositoryClient)
	{
		_repositoryClient = repositoryClient;
	}

	public async Task<WriteMessageResultModel> WriteMessageAsync(MessageEntity message)
	{
		WriteMessageResultModel? response = await _repositoryClient.WriteMessageToDbAsync(new WriteMessageDbRequest
		{
			RequestUserId = message.IssuerId,
			ResponseUserId = message.ReceiverId,
			TextMessage = message.TextMessage,
			MessageId = message.MessageId
		});

		if (response is null || response.IsSuccessful is false)
		{
			throw new Exception("Failed to write message");
		}

		return response;
	}

	public async Task<IAsyncEnumerable<MessageFromDb>> GetUserMessagesByUserIdAsync(int userId)
	{
		await Task.Delay(100); //TODO: remove it in prod

		AsyncServerStreamingCall<MessageFromDb>? response = _repositoryClient.GetUserMessagesFromDb(
			new GetMessagesDbRequest
			{
				RequestUserId = userId
			});

		IAsyncEnumerable<MessageFromDb> messagesFromDb = response.ResponseStream.ReadAllAsync();

		return messagesFromDb;
	}
}