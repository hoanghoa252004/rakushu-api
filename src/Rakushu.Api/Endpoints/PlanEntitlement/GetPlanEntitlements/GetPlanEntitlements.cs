using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.PlanEntitlement.GetPlanEntitlements;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.PlanEntitlement.GetPlanEntitlements;

internal sealed class GetPlanEntitlements : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEntitlementEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[FromRoute] Guid planId,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetPlanEntitlementsQuery(planId);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetPlanEntitlements")
			.WithDescription("Retrieves all feature entitlements for the specified plan.")
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces<IReadOnlyCollection<PlanEntitlementDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
