using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Subscription.GetMySubscriptions;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptionById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Subscription.GetMySubscriptions;

internal sealed class GetMySubscriptions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSubscriptionEndpoints()
			// 1. Endpoint
			.MapGet("/me", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] Guid? planId,
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetMySubscriptionsQuery(
					pagination.PageNumber,
					pagination.PageSize,
					planId,
					status);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetMySubscriptions")
			.WithSummary("Learner")
			.WithDescription("Retrieves all subscriptions of the authenticated user with optional filtering.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			// 4. Response
			.Produces<PaginatedList<SubscriptionDetailDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
