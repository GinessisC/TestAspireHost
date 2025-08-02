using Contracts.Requests;

namespace Storefront.Extensions.Mapping;

public static class RequestsExtensions
{
	public static WriteMessageRequest MapToGrpcRequest(this WriteMessageRestRequest request)
	{
		return new WriteMessageRequest
		{
			RequestUserId = request.RequestUserId,
			ResponseUserId = request.ResponseUserId,
			TextMessage = request.TextMessage
		};
	}
}