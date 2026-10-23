using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.UpdateKnowledgeUnit;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeUnit.UpdateKnowledgeUnit;

internal sealed class UpdateKnowledgeUnit : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeUnitEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateKnowledgeUnitRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateKnowledgeUnitCommand(
					id,
					dto.KnowledgeCollectionId,
					dto.ProficiencyLevelId,
					dto.KnowledgeType,
					dto.Expression,
					dto.Reading,
					dto.IsActive,
					dto.AudioKey
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateKnowledgeUnit")
			.WithSummary("Curator")
			.WithDescription("Update a knowledge unit.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateKnowledgeUnitRequestDto(
	Guid KnowledgeCollectionId,
	Guid ProficiencyLevelId,
	KnowledgeUnitType KnowledgeType,
	string Expression,
	string Reading,
	bool IsActive,
	string? AudioKey = null
	);
