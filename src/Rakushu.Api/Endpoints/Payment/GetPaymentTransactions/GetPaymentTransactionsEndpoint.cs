using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Payment.GetPaymentTransactions;

namespace Rakushu.Api.Endpoints.Payment.GetPaymentTransactions;

internal sealed class GetPaymentTransactionsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapPaymentEndpoints();

		var handler = async (
			[FromRoute] Guid id,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetPaymentTransactionsQuery(id);
			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		};

		// Primary plural route
		group.MapGet("/{id:guid}/transactions", handler)
			.WithName("GetPaymentTransactions")
			.WithDescription("Retrieves all transactions associated with a payment.")
			.RequireAuthorization()
			.Produces<IReadOnlyCollection<PaymentTransactionDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		// Backward-compatible singular alias
		group.MapGet("/{id:guid}/transaction", handler)
			.ExcludeFromDescription();
	}
}
