using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Feature.CreateFeature;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Feature.CreateFeature;

internal sealed class CreateFeature : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFeatureEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateFeatureRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateFeatureCommand(
					dto.Code,
					dto.Name,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetFeatureById", featureId => new { id = featureId });
			})
			// 2. Description
			.WithName("CreateFeature")
			.WithDescription("Creates a new feature with the specified details.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateFeatureRequestDto(
	string Code = "AI_CHAT",
	string Name = "AI Chat Feature",
	string? Description = null
);

