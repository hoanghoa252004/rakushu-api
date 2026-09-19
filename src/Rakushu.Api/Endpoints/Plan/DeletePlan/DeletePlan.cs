using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Plan.DeletePlan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Plan.DeletePlan;

internal sealed class DeletePlan : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeletePlanCommand(id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithTags("Plan")
			.WithName("DeletePlan")
			.WithDescription("Deletes a plan. Cannot delete a plan that has subscriptions.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
