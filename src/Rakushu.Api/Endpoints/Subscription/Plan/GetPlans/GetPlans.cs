using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlans;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Plan.GetPlans;

internal sealed class GetPlans : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetPlansQuery();

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetPlans")
			.WithSummary("AllowAnonymous")
			.WithDescription("Retrieves a paginated list of all plans with their details.")
			// 3.Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<IReadOnlyCollection<PlanDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
