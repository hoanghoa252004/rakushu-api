using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.UpdateKnowledgeMeaning;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeMeaning.UpdateKnowledgeMeaning;

internal sealed class UpdateKnowledgeMeaning : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeMeaningEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateKnowledgeMeaningRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateKnowledgeMeaningCommand(
					id,
					dto.SortOrder,
					dto.Meaning,
					dto.JapaneseMeaning,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateKnowledgeMeaning")
			.WithSummary("Curator")
			.WithDescription("Update a knowledge meaning.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateKnowledgeMeaningRequestDto(
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description = null
	);
