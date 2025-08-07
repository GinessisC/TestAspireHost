using DataProvider.Abstractions.Repositories;
using DataProvider.Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.Services;

public class AppService : MessengerRepository.MessengerRepositoryBase
{
	private readonly IRepository<UserEntity> _userRepository;
	private readonly IRepository<MessageEntity> _messageRepository;
	private readonly ILogger<AppService> _logger;
	private const int TakeCount = 10;
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
		bool isRequestValid = await BothUsersExistAsync(request, context.CancellationToken);

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
		}, context.CancellationToken);

		return response;
	}

	public override async Task GetUserMessagesFromDb(GetMessagesDbRequest request,
		IServerStreamWriter<MessageFromDb> responseStream,
		ServerCallContext context)
	{
		IQueryable<MessageFromDb> messagesForRequestedUser = _messageRepository
			.Where(m => m.ReceiverId == request.RequestUserId)
			.Select(m => new MessageFromDb()
			{
				MessageId = m.MessageId,
				Message = m.MessageText,
				UserId = m.SenderId
			}).OrderBy(e => e.MessageId)
			.Take(TakeCount)
			.Reverse();
		
		await foreach (MessageFromDb message in messagesForRequestedUser.AsAsyncEnumerable())
		{
			await responseStream.WriteAsync(message);
		}
	}

	private async Task<bool> BothUsersExistAsync(WriteMessageDbRequest request, CancellationToken ct)
	{
		bool requestUserExistsInDb =
			await _userRepository.FirstOrDefaultAsync(u => u.UserId == request.RequestUserId, ct) is not null;

		bool responseUserExistsInDb =
			await _userRepository.FirstOrDefaultAsync(u => u.UserId == request.ResponseUserId, ct) is not null;

		bool bothUsersWereFound = requestUserExistsInDb && responseUserExistsInDb;

		_logger.LogTrace($"Both users have been found: {bothUsersWereFound}");

		return bothUsersWereFound;
	}
}