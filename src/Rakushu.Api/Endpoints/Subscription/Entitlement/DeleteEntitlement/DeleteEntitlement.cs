using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Entitlement.DeleteEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Entitlement.DeleteEntitlement;

internal sealed class DeleteEntitlement : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapEntitlementEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid planId,
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeleteEntitlementCommand(planId, id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("DeleteEntitlement")
			.WithDescription("Deletes an entitlement ( Detach a feature ) from the specified plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
