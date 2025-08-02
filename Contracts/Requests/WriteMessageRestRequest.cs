namespace Contracts.Requests;

public sealed record WriteMessageRestRequest(
	int RequestUserId,
	int ResponseUserId,
	string TextMessage);