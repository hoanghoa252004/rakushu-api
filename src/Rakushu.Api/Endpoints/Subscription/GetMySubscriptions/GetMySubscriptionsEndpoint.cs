using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.GetMySubscriptions;

namespace Rakushu.Api.Endpoints.Subscription.GetMySubscriptions;

internal sealed class GetMySubscriptionsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSubscriptionEndpoints()
			.MapGet("/my-history", async (
				[AsParameters] PaginationRequest pagination,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetMySubscriptionsQuery(pagination.PageNumber, pagination.PageSize);
				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetMySubscriptions")
			.WithDescription("Retrieves the authenticated user's subscription history.")
			.RequireAuthorization()
			.Produces<PaginatedList<SubscriptionSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
