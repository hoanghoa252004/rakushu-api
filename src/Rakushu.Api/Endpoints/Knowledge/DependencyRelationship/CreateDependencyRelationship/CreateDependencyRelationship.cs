using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Create;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.DependencyRelationship.CreateDependencyRelationship;

internal sealed class CreateDependencyRelationship : IEndpoint
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
			.WithSummary("Curator")
			.WithDescription("Creates a new dependency relationship with Vietnamese and Japanese names.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces<Guid>(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateDependencyRelationshipRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? Description = null);
