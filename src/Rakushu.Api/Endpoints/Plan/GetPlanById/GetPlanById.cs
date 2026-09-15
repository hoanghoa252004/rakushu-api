using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Plan.GetPlanById;

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
			.WithTags("Plan")
			.WithName("GetPlanById")
			.WithDescription("Retrieves a specific plan by its ID.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<PlanDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
