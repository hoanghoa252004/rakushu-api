namespace Rakushu.Api.Endpoints.Transaction;

internal static class TransactionEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/payments")
			.WithTags("Transaction")
			.WithGroupName("subscription");
	}
}
