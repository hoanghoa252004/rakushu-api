using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class SubmitFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPost("/flashcard-sessions/{id:guid}/submit", async (
				[FromRoute] Guid id,
				[FromBody] SubmitFlashcardSessionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SubmitFlashcardSessionCommand(id, dto.Ratings);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("SubmitFlashcardSession")
			.WithSummary("Submit flashcard self-evaluation ratings and update SM-2 schedule")
			.WithDescription("Submits ratings ('Forgot', 'Hard', 'Easy') for all flashcard items, calculates SM-2 spaced repetition intervals, and records review history.")
			.RequireAuthorization()
			.Produces<FlashcardSessionResultDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SubmitFlashcardSessionRequestDto(
	IReadOnlyList<FlashcardItemRatingInput> Ratings);
