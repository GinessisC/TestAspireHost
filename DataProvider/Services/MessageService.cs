using System.Linq.Expressions;
using DataProvider.Abstractions.Repositories;
using DataProvider.Entities;

namespace DataProvider.Services;

public class MessageService
{
	private readonly IRepository<MessageEntity> _messageRepository;

	public MessageService(IRepository<MessageEntity> messageRepository)
	{
		_messageRepository = messageRepository;
	}

	public async Task AddMessageAsync(MessageEntity message)
	{
		await _messageRepository.AddAsync(message);
	}

	public IQueryable<MessageEntity> Where(Expression<Func<MessageEntity, bool>> predicate)
	{
		return _messageRepository.Where(predicate);
	}
}