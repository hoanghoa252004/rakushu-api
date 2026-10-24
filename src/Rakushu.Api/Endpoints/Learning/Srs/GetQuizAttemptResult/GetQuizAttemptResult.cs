using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.GetQuizAttemptResult;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResult : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapGet(
				"/quiz-attempts/{attemptId:guid}/result",
				async (
					Guid attemptId,
					ISender sender,
					CancellationToken cancellationToken) =>
				{
					var result = await sender.Send(
						new GetQuizAttemptResultQuery(attemptId),
						cancellationToken);

					return result.MatchOk();
				})
			// 2. Description
			.WithName("GetSrsQuizAttemptResult")
			.WithSummary("Retrieve a completed quiz result")
			.WithDescription("Retrieves the detailed score, answered options, feedback, and next SM-2 review schedules for a completed quiz attempt.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<QuizAttemptResultResponse>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
