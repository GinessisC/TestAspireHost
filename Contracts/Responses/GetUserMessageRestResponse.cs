namespace Contracts.Responses;

public sealed record GetUserMessageRestResponse(
	string MessageId,
	string TextMessage,
	int ReceiverId);