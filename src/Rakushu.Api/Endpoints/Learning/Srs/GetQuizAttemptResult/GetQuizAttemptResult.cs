using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.GetQuizAttemptResult;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResult : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapGet("/quiz-attempts/{attemptId:guid}/result", async (
				Guid attemptId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetQuizAttemptResultQuery(attemptId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsQuizAttemptResult")
			.WithSummary("Retrieve detailed results and explanations for a completed quiz attempt")
			.WithDescription("Retrieves the score, answered options, correct answers, and curated explanations from bank questions.")
			.RequireAuthorization()
			.Produces<QuizAttemptResultResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
