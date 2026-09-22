using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.CancelPayment;

namespace Rakushu.Api.Endpoints.Payment.CancelPayment;

internal sealed class CancelPaymentEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			.MapPost("/{id:guid}/cancel", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CancelPaymentCommand(id);
				var result = await sender.Send(command, cancellationToken);

				return result.MatchNoContent();
			})
			.WithName("CancelPayment")
			.WithDescription("Cancels an active pending payment order and marks associated subscription as failed.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
