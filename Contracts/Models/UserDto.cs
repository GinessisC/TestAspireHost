namespace Contracts.Models;

public sealed record UserDto(
	int UserId,
	string UserName,
	string Description);