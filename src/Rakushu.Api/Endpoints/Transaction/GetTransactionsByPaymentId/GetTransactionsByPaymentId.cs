using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Transaction.GetTransactionsByPaymentId;

namespace Rakushu.Api.Endpoints.Transaction.GetTransactionsByPaymentId;

internal sealed class GetTransactionsByPaymentId : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTransactionEndpoints()
			// 1. Endpoint
			.MapGet("/{paymentId:guid}/transactions", async (
					[FromRoute] Guid paymentId,
					[AsParameters] PaginationRequest pagination,
					[FromQuery] string? status,
					[FromQuery] string? provider,
					ISender sender,
					CancellationToken cancellationToken
					) =>
				{
					var query = new GetTransactionsByPaymentIdQuery(
						paymentId,
						status,
						provider);

					var result = await sender.Send(query, cancellationToken);

					return result.MatchOk();
				})
			// 2. Description
			.WithName("GetTransactionsByPaymentId")
			.WithDescription("Retrieves all transactions for the specified payment with optional filtering.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<IReadOnlyCollection<TransactionDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
