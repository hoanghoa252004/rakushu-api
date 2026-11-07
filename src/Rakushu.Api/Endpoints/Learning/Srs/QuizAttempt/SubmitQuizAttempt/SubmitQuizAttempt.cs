using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

internal sealed class SubmitQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/quiz-attempts/{id:guid}/submit", async (
				Guid id,
				[FromBody] SubmitQuizAttemptRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var answers = (dto.Answers ?? [])
					.Select(a => new QuizAnswerSubmission(a.QuestionId, a.SelectedOptionId, a.ElapsedMs))
					.ToList();

				var command = new SubmitQuizAttemptCommand(id, answers);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("SubmitSrsQuizAttempt")
			.WithSummary("Learner")
			.WithDescription("Submits learner answers, scores questions, applies SuperMemo-2 quality tiers (1: unanswered, 2: incorrect, 3-5: speed tiers), updates schedules and logs audit details.")
			.RequireAuthorization()
			.Produces<QuizAttemptResultResponse>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SubmitAnswerRequestDto(
	Guid QuestionId,
	string? SelectedOptionId = null,
	int? ElapsedMs = null);

internal sealed record SubmitQuizAttemptRequestDto(
	IReadOnlyList<SubmitAnswerRequestDto>? Answers = null);
