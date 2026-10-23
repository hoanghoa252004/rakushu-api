using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.CreateKnowledgeCollection;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeCollection.CreateKnowledgeCollection;

internal sealed class CreateKnowledgeCollection : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeCollectionEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateKnowledgeCollectionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateKnowledgeCollectionCommand(
					dto.Code,
					dto.Name,
					dto.CollectionType,
					dto.Scope,
					dto.IsActive,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetKnowledgeCollectionById", id => new { id });
			})
			// 2. Description
			.WithName("CreateKnowledgeCollection")
			.WithSummary("Curator")
			.WithDescription("Create a knowledge collection.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateKnowledgeCollectionRequestDto(
	string Code,
	string Name,
	KnowledgeCollectionType CollectionType,
	KnowledgeCollectionScope Scope,
	bool IsActive,
	string? Description = null
	);
