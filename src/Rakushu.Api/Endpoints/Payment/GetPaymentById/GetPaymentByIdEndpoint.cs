using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.GetPaymentById;

namespace Rakushu.Api.Endpoints.Payment.GetPaymentById;

internal sealed class GetPaymentByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetPaymentByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetPaymentById")
			.WithDescription("Retrieves the payment status, QR code, and transaction details by payment ID.")
			.RequireAuthorization()
			.Produces<PaymentDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
