using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttemptAnswer;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.SubmitQuizAttemptAnswer;

internal sealed class SubmitQuizAttemptAnswer : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPut("/{attemptId:guid}/items/{itemId:guid}/answer", async (
				[FromRoute] Guid attemptId,
				[FromRoute] Guid itemId,
				[FromBody] SubmitQuizAttemptAnswerRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SubmitQuizAttemptAnswerCommand(
					attemptId,
					itemId,
					dto.SelectedAnswer,
					dto.TimeSpentMs);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("SubmitQuizAttemptAnswer")
			.WithSummary("Learner")
			.WithDescription("Submits learner answer for a single quiz item and records elapsed time. Final evaluation and SM-2 spaced repetition updates are applied upon submission. Pass 'SKIP' as SelectedAnswer to skip.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SubmitQuizAttemptAnswerRequestDto(
	string SelectedAnswer,
	int TimeSpentMs);
