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

	public async IAsyncEnumerable<MessageFromDb> GetUserMessagesByUserIdAsync(int userId, CancellationToken ct)
	{

		AsyncServerStreamingCall<MessageFromDb>? response = _repositoryClient.GetUserMessagesFromDb(
			new GetMessagesDbRequest
			{
				RequestUserId = userId
			});
		
		//TODO: is it the best way to read ALL from stream?
		//IAsyncEnumerable<MessageFromDb> messagesFromDb = response.ResponseStream.ReadAllAsync(ct);

		while (await response.ResponseStream.MoveNext(ct))
		{
			var currentMessage = response.ResponseStream.Current;
			yield return currentMessage;
		}
	}
}