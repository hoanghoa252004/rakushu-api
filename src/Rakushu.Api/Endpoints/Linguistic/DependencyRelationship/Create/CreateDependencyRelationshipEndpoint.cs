using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Create;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.Create;

internal sealed class CreateDependencyRelationshipEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapPost("/", async (
				[FromBody] CreateDependencyRelationshipRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateDependencyRelationshipCommand(
					dto.Code,
					dto.Name,
					dto.JapaneseName,
					dto.Description);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetDependencyRelationshipById", id => new { id });
			})
			.WithName("CreateDependencyRelationship")
			.WithDescription("Creates a new dependency relationship with Vietnamese and Japanese names.")
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateDependencyRelationshipRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? Description = null);
