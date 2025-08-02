using DataProvider.Abstractions.Repositories;
using DataProvider.Entities;

namespace DataProvider.Services;

//TODO: 
public class UserService
{
	private readonly IRepository<UserEntity> _userRepository;

	public UserService(IRepository<UserEntity> userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<bool> Exists(int userId)
	{
		UserEntity? user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == userId);

		return user is not null;
	}
}