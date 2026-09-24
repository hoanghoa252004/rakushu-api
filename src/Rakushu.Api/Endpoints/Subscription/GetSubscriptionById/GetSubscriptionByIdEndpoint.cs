using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.GetSubscriptionById;

namespace Rakushu.Api.Endpoints.Subscription.GetSubscriptionById;

internal sealed class GetSubscriptionByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSubscriptionEndpoints()
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSubscriptionByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetSubscriptionById")
			.WithDescription("Retrieves the details of a specific subscription.")
			.RequireAuthorization()
			.Produces<SubscriptionDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
