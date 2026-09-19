using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Feature.UpdateFeature;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Feature.UpdateFeature;

internal sealed class UpdateFeature : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFeatureEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateFeatureRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateFeatureCommand(
					id,
					dto.Name,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateFeature")
			.WithDescription("Updates an existing feature with the provided details.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateFeatureRequestDto(
	string Name,
	string? Description = null
);

