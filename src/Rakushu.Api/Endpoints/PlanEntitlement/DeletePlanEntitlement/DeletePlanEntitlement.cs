using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.PlanEntitlement.DeletePlanEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.PlanEntitlement.DeletePlanEntitlement;

internal sealed class DeletePlanEntitlement : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEntitlementEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid planId,
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeletePlanEntitlementCommand(planId, id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("DeletePlanEntitlement")
			.WithDescription("Deletes a feature entitlement from the specified plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
