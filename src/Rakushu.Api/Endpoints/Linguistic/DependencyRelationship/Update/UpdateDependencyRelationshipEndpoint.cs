using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Update;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.Update;

internal sealed class UpdateDependencyRelationshipEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapPut("/{id}", async (Guid id, [FromBody] UpdateDependencyRelationshipRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new UpdateDependencyRelationshipCommand(id, dto.Name, dto.JapaneseName, dto.Description);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateDependencyRelationship")
			.WithDescription("Updates an existing dependency relationship.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateDependencyRelationshipRequestDto(
	string Name,
	string JapaneseName,
	string? Description);
