using Contracts.Models;
using Contracts.Responses;

namespace Storefront.Extensions.Mapping;

public static class ResponseMapping
{
	public static GetUserMessageRestResponse MapToRestResponse(this GetMessageResponse message)
	{
		return new GetUserMessageRestResponse(message.MessageId, message.Message, message.ReceiverId);
	}

	private static UserDto MapToRestUser(this UserDto userDto)
	{
		return new UserDto(userDto.UserId, userDto.UserName, userDto.Description);
	}
}