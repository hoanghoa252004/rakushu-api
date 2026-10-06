using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Delete;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.Delete;

internal sealed class DeleteDependencyRelationshipEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteDependencyRelationshipCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteDependencyRelationship")
			.WithDescription("Deletes an existing dependency relationship.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
