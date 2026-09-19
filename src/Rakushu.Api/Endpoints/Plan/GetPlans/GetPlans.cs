using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Plan.GetPlans;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Api.Endpoints.Plan.GetPlans;

internal sealed class GetPlans : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] PlanStatus? status,
				[FromQuery] Guid[]? FeatureIds,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetPlansQuery(
					pagination.PageNumber, 
					pagination.PageSize,
					FeatureIds,
					status);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithTags("Plan")
			.WithName("GetPlans")
			.WithDescription("Retrieves a paginated list of all plans with their details.")
			// 3.Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
