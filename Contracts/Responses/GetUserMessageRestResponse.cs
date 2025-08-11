namespace Contracts.Responses;

public sealed record GetUserMessageRestResponse(
	int MessageId,
	string TextMessage,
	int ReceiverId);