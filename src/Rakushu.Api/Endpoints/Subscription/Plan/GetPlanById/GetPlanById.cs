using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Subscription.Plan;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Plan.GetPlanById;

internal sealed class GetPlanById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetPlanByIdQuery(id);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetPlanById")
			.WithSummary("AllowAnonymous")
			.WithDescription("Retrieves a specific plan by its ID.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<PlanDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
