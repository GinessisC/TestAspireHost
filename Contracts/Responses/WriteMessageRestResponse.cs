namespace Contracts.Responses;

public sealed record WriteMessageRestResponse(
	bool IsSuccessful,
	string? ErrorMessage);