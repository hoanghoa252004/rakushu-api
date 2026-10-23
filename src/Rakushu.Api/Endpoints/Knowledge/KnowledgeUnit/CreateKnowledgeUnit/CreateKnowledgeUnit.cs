using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.CreateKnowledgeUnit;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeUnit.CreateKnowledgeUnit;

internal sealed class CreateKnowledgeUnit : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeUnitEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateKnowledgeUnitRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateKnowledgeUnitCommand(
					dto.KnowledgeCollectionId,
					dto.ProficiencyLevelId,
					dto.KnowledgeType,
					dto.Expression,
					dto.Reading,
					dto.IsActive,
					dto.AudioKey
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetKnowledgeUnitById", id => new { id });
			})
			// 2. Description
			.WithName("CreateKnowledgeUnit")
			.WithSummary("Curator")
			.WithDescription("Create a knowledge unit.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateKnowledgeUnitRequestDto(
	Guid KnowledgeCollectionId,
	Guid ProficiencyLevelId,
	KnowledgeUnitType KnowledgeType,
	string Expression,
	string Reading,
	bool IsActive,
	string? AudioKey = null
	);
