using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Update;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.Update;

internal sealed class UpdateDependencyRelationship : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id, 
				[FromBody] UpdateDependencyRelationshipRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateDependencyRelationshipCommand(id, dto.Name, dto.JapaneseName, dto.Description);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateDependencyRelationship")
			.WithSummary("Admin")
			.WithDescription("Updates an existing dependency relationship.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateDependencyRelationshipRequestDto(
	string Name,
	string JapaneseName,
	string? Description);
