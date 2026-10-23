using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.CreateKnowledgeMeaning;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeMeaning.CreateKnowledgeMeaning;

internal sealed class CreateKnowledgeMeaning : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeMeaningEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateKnowledgeMeaningRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateKnowledgeMeaningCommand(
					dto.KnowledgeUnitId,
					dto.SortOrder,
					dto.Meaning,
					dto.JapaneseMeaning,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetKnowledgeMeaningById", id => new { id });
			})
			// 2. Description
			.WithName("CreateKnowledgeMeaning")
			.WithSummary("Curator")
			.WithDescription("Create a knowledge meaning for a knowledge unit.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateKnowledgeMeaningRequestDto(
	Guid KnowledgeUnitId,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description = null
	);
