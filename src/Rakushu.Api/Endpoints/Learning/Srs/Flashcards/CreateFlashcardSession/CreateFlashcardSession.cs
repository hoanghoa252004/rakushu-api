using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.CreateFlashcardSession;

internal sealed class CreateFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPost("/", async (
				[FromBody] CreateFlashcardSessionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateFlashcardSessionCommand(dto.ItemIds, dto.DiscardActive);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetFlashcardSession", sessionId => new { sessionId });
			})
			.WithName("CreateFlashcardSession")
			.WithSummary("Learner")
			.WithDescription("Initializes a new flashcard review session for user-selected vocabulary items.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateFlashcardSessionRequestDto(
	IReadOnlyList<Guid> ItemIds,
	bool DiscardActive = false);
