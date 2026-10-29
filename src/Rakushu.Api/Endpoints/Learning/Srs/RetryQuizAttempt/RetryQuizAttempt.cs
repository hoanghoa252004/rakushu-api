using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/quiz-attempts/{id:guid}/retry", async (
				Guid id,
				[FromBody] RetryQuizAttemptRequestDto? dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var retryMode = dto?.RetryMode ?? "All";
				var command = new RetryQuizAttemptCommand(id, retryMode);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetSrsQuizAttemptResult", resp => new { attemptId = resp.AttemptId });
			})
			.WithName("RetrySrsQuizAttempt")
			.WithSummary("Retry a completed quiz attempt (All questions or MistakesOnly)")
			.WithDescription("Creates a new practice attempt from an existing completed quiz. Practice attempts run in PracticeAll or PracticeMistakes mode and do not alter SM-2 metrics.")
			.RequireAuthorization()
			.Produces<QuizAttemptStartedResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record RetryQuizAttemptRequestDto(string? RetryMode = "All");
