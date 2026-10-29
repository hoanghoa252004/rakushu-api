using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.CreateFlashcardSession;

internal sealed class CreateFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPost("/flashcard-sessions", async (
				[FromBody] CreateFlashcardSessionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateFlashcardSessionCommand(dto.ItemIds);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetFlashcardSession", resp => new { id = resp.SessionId });
			})
			.WithName("CreateFlashcardSession")
			.WithSummary("Create a flashcard study session with video context and subtitles")
			.WithDescription("Initializes a new flashcard review session for user-selected vocabulary items, embedding corresponding video segments and bilingual subtitles.")
			.RequireAuthorization()
			.Produces<FlashcardSessionDetailDto>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateFlashcardSessionRequestDto(
	IReadOnlyList<Guid> ItemIds);
