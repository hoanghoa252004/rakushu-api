using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.GetSubscriptions;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.GetSubscriptions;

internal sealed class GetSubscriptionsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSubscriptionEndpoints()
			.MapGet(string.Empty, async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? status,
				[FromQuery] Guid? planId,
				[FromQuery] Guid? userId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSubscriptionsQuery(
					pagination.PageNumber,
					pagination.PageSize,
					status,
					planId,
					userId);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetSubscriptions")
			.WithDescription("Retrieves a paginated list of all subscriptions with optional filtering (Admin only).")
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			.Produces<PaginatedList<SubscriptionAdminDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
