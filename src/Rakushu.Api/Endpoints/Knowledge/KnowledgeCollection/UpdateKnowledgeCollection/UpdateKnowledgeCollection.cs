using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.UpdateKnowledgeCollection;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeCollection.UpdateKnowledgeCollection;

internal sealed class UpdateKnowledgeCollection : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeCollectionEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateKnowledgeCollectionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateKnowledgeCollectionCommand(
					id,
					dto.Name,
					dto.CollectionType,
					dto.Scope,
					dto.IsActive,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateKnowledgeCollection")
			.WithSummary("Curator")
			.WithDescription("Update a knowledge collection.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateKnowledgeCollectionRequestDto(
	string Name,
	KnowledgeCollectionType CollectionType,
	KnowledgeCollectionScope Scope,
	bool IsActive,
	string? Description = null
	);
