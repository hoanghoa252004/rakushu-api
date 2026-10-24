using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapPost("/quiz-attempts/{attemptId:guid}/retry", async (
				Guid attemptId,
				[FromBody] RetryQuizAttemptRequestDto? dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var retryMode = dto?.RetryMode ?? "All";
				var command = new RetryQuizAttemptCommand(attemptId, retryMode);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("RetrySrsQuizAttempt")
			.WithSummary("Retry a completed quiz attempt (All questions or MistakesOnly)")
			.WithDescription(
				"Creates a new practice quiz attempt from an existing completed attempt. " +
				"Supported modes: 'All' (retries all questions) and 'MistakesOnly' (retries only incorrect questions). " +
				"Practice attempts do not modify SRS SM-2 scheduling metrics.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<QuizAttemptStartedResponse>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record RetryQuizAttemptRequestDto(
	string? RetryMode = "All");
