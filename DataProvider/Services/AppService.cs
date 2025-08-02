using DataProvider.Abstractions.Repositories;
using DataProvider.Entities;
using Grpc.Core;

namespace DataProvider.Services;

public class AppService : MessengerRepository.MessengerRepositoryBase
{
	private readonly IRepository<UserEntity> _userRepository;
	private readonly IRepository<MessageEntity> _messageRepository;

	private readonly ILogger<AppService> _logger;

	public AppService(
		ILogger<AppService> logger,
		IRepository<UserEntity> userRepository,
		IRepository<MessageEntity> messageRepository)
	{
		_logger = logger;
		_userRepository = userRepository;
		_messageRepository = messageRepository;
	}

	public override async Task<WriteMessageResultModel> WriteMessageToDb(
		WriteMessageDbRequest request,
		ServerCallContext context)
	{
		bool isRequestValid = await BothUsersExistAsync(request);

		WriteMessageResultModel response = new WriteMessageResultModel
		{
			IsSuccessful = isRequestValid
		};

		if (isRequestValid is false)
		{
			response.ErrorDescription = $"UserEntity {request.RequestUserId} not found";
		}

		await _messageRepository.AddAsync(new MessageEntity
		{
			MessageId = request.MessageId,
			SenderId = request.RequestUserId,
			ReceiverId = request.ResponseUserId,
			MessageText = request.TextMessage
		});

		return response;
	}

	public override async Task GetUserMessagesFromDb(GetMessagesDbRequest request,
		IServerStreamWriter<MessageFromDb> responseStream,
		ServerCallContext context)
	{
		IQueryable<MessageEntity> messagesForRequestedUser = _messageRepository
			.Where(m => m.ReceiverId == request.RequestUserId);

		_logger.LogInformation($"{messagesForRequestedUser.Count()} messages found");

		foreach (MessageEntity message in messagesForRequestedUser)
		{
			await responseStream.WriteAsync(new MessageFromDb
			{
				Message = message.MessageText,
				MessageId = message.MessageId,
				UserId = message.SenderId
			});
		}
	}

	private async Task<bool> BothUsersExistAsync(WriteMessageDbRequest request)
	{
		bool requestUserExistsInDb =
			await _userRepository.FirstOrDefaultAsync(u => u.UserId == request.RequestUserId) is not null;

		bool responseUserExistsInDb =
			await _userRepository.FirstOrDefaultAsync(u => u.UserId == request.ResponseUserId) is not null;

		bool bothUsersWereFound = requestUserExistsInDb && responseUserExistsInDb;

		_logger.LogTrace($"Both users have been found: {bothUsersWereFound}");

		return bothUsersWereFound;
	}
}