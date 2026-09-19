using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Feature.DeleteFeature;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Feature.DeleteFeature;

internal sealed class DeleteFeature : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFeatureEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeleteFeatureCommand(id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("DeleteFeature")
			.WithDescription("Deletes a feature. Cannot delete a feature that has subscription usage.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
