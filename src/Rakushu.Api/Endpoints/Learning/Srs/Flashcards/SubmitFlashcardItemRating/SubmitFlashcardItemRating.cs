using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardItemRating;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.SubmitFlashcardItemRating;

internal sealed class SubmitFlashcardItemRating : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPut("/{sessionId:guid}/items/{itemId:guid}/rating", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid itemId,
				[FromBody] SubmitFlashcardItemRatingInput input,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SubmitFlashcardItemRatingCommand(sessionId, itemId, input.Rating);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("SubmitFlashcardItemRating")
			.WithSummary("Learner")
			.WithDescription("Submits rating ('Forgot', 'Hard', 'Easy') for an individual flashcard item, calculates SM-2 spaced repetition intervals, and records review history.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
