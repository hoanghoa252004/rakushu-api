using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.CancelPaymentTransaction;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;

namespace Rakushu.Api.Endpoints.Payment.CancelPaymentTransaction;

internal sealed class CancelPaymentTransactionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapPaymentEndpoints();

		var postCancelHandler = async (
			[FromRoute] Guid id,
			[FromRoute] Guid transactionId,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new CancelPaymentTransactionCommand(id, transactionId);
			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		};

		var patchStatusHandler = async (
			[FromRoute] Guid id,
			[FromRoute] Guid transactionId,
			[FromBody] ChangePaymentTransactionStatusRequestDto dto,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			if (!string.Equals(dto.Status, nameof(TransactionStatus.Canceled), StringComparison.OrdinalIgnoreCase))
			{
				return Result.Failure(PaymentErrors.TransactionCannotBeCanceled).MatchOk();
			}

			var command = new CancelPaymentTransactionCommand(id, transactionId);
			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		};

		// POST /api/payments/{id:guid}/transactions/{transactionId:guid}/cancel (Task-based endpoint)
		group.MapPost("/{id:guid}/transactions/{transactionId:guid}/cancel", postCancelHandler)
			.WithName("CancelPaymentTransaction")
			.WithDescription("Cancels an individual payment transaction.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		// Backward-compatible POST singular alias
		group.MapPost("/{id:guid}/transaction/{transactionId:guid}/cancel", postCancelHandler)
			.ExcludeFromDescription();

		// PATCH /api/payments/{id:guid}/transactions/{transactionId:guid}/status (Plural)
		group.MapPatch("/{id:guid}/transactions/{transactionId:guid}/status", patchStatusHandler)
			.WithName("ChangePaymentTransactionStatus")
			.WithDescription("Updates the status of an individual payment transaction (e.g. Canceled). Deprecated: use POST cancel.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		// Backward-compatible PATCH singular alias
		group.MapPatch("/{id:guid}/transaction/{transactionId:guid}/status", patchStatusHandler)
			.ExcludeFromDescription();
	}
}

internal sealed record ChangePaymentTransactionStatusRequestDto(string Status);
