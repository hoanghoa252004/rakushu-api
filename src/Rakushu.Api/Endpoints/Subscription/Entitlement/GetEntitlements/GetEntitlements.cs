using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Entitlement.GetEntitlements;

internal sealed class GetEntitlements : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapEntitlementEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[FromRoute] Guid planId,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetEntitlementsQuery(planId);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetEntitlements")
			.WithSummary("Admin")
			.WithDescription("Retrieves all feature entitlements for the specified plan.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<IReadOnlyCollection<EntitlementDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
