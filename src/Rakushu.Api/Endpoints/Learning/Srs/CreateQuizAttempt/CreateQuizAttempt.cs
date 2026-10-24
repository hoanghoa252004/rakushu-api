using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapPost("/quiz-attempts", async (
				[FromBody] CreateQuizAttemptRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateQuizAttemptCommand(
					dto.ItemIds,
					dto.QuestionFormat ?? "MC");

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("CreateSrsQuizAttempt")
			.WithSummary("Generate an MC quiz from selected SRS items")
			.WithDescription(
				"Provide selected SRS item IDs and QuestionFormat (MC). Generates multiple-choice quiz questions with a balanced mix of " +
				"Japanese-to-Vietnamese and Vietnamese-to-Japanese directions, with 1 correct answer and 3 random distractors from the knowledge base. " +
				"Upon submission, SM-2 calculates the interval and next review date, and LLM generates adaptive feedback.")
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

internal sealed record CreateQuizAttemptRequestDto(
	IReadOnlyList<Guid>? ItemIds,
	string? QuestionFormat = "MC");
