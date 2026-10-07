using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Subscription.GetSubscriptions;

internal sealed class GetSubscriptions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSubscriptionEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] Guid? userId,
				[FromQuery] Guid? planId,
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetSubscriptionsQuery(
					pagination.PageNumber,
					pagination.PageSize,
					userId,
					planId,
					status);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetSubscriptions")
			.WithSummary("Admin")
			.WithDescription("Retrieves a paginated list of all subscriptions with optional filtering.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<PaginatedList<SubscriptionDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
