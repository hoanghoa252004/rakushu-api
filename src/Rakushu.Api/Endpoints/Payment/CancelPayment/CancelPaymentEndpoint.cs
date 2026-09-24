using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.CancelPayment;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Api.Endpoints.Payment.CancelPayment;

internal sealed class CancelPaymentEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapPaymentEndpoints();

		// POST /api/payments/{id:guid}/cancel (Task-based endpoint)
		group.MapPost("/{id:guid}/cancel", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CancelPaymentCommand(id);
				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("CancelPayment")
			.WithDescription("Cancels a pending payment.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		// PATCH /api/payments/{id:guid}/status (Backward-compatible alias)
		group.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangePaymentStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				if (!string.Equals(dto.Status, nameof(PaymentStatus.Canceled), StringComparison.OrdinalIgnoreCase))
				{
					return Result.Failure(PaymentErrors.CannotCancel).MatchOk();
				}

				var command = new CancelPaymentCommand(id);
				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("ChangePaymentStatus")
			.WithDescription("Updates the status of a payment by ID (e.g. Canceled). Deprecated: use POST /api/payments/{id}/cancel.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ChangePaymentStatusRequestDto(string Status);
