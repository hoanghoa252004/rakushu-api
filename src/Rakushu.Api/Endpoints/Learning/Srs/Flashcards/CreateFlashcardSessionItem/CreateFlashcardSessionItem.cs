using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSessionItem;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.CreateFlashcardSessionItem;

internal sealed class CreateFlashcardSessionItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPost("/{sessionId:guid}/items", async (
				[FromRoute] Guid sessionId,
				[FromBody] CreateFlashcardSessionItemRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateFlashcardSessionItemCommand(sessionId, dto.SavedItemId, dto.Rating);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetFlashcardSession", _ => new { sessionId });
			})
			.WithName("CreateFlashcardSessionItem")
			.WithSummary("Learner")
			.WithDescription("Records a rating ('Forgot', 'Hard', 'Easy') for an individual flashcard item during an active session, creating the session item record.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateFlashcardSessionItemRequestDto(
	Guid SavedItemId,
	string Rating);

