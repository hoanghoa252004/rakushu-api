using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Delete;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.DependencyRelationship.DeleteDependencyRelationship;

internal sealed class DeleteDependencyRelationship : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapDelete("/{id:guid}", async ([FromRoute]Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteDependencyRelationshipCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteDependencyRelationship")
			.WithSummary("Curator")
			.WithDescription("Deletes an existing dependency relationship.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
