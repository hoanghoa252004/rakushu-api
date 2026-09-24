using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.CancelSubscription;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Api.Endpoints.Subscription.CancelSubscription;

internal sealed class CancelSubscriptionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapSubscriptionEndpoints();

		// POST /api/subscriptions/{id:guid}/cancel (Task-based endpoint)
		group.MapPost("/{id:guid}/cancel", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CancelSubscriptionCommand(id);
				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("CancelSubscription")
			.WithDescription("Cancels an active subscription.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		// PATCH /api/subscriptions/{id:guid}/status (Backward-compatible alias)
		group.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangeSubscriptionStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				if (!string.Equals(dto.Status, nameof(SubscriptionStatus.Canceled), StringComparison.OrdinalIgnoreCase))
				{
					return Result.Failure(SubscriptionErrors.InvalidStatus).MatchOk();
				}

				var command = new CancelSubscriptionCommand(id);
				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("ChangeSubscriptionStatus")
			.WithDescription("Updates the status of a subscription (e.g. Canceled). Deprecated: use POST /api/subscriptions/{id}/cancel.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ChangeSubscriptionStatusRequestDto(string Status);
