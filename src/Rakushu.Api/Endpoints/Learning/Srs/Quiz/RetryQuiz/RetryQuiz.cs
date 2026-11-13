using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.Quiz.RetryQuiz;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Quiz.RetryQuiz;

internal sealed class RetryQuiz : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/{quizId:guid}/retry", async (
				[FromRoute] Guid quizId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new RetryQuizCommand(quizId);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetQuizAttempt", attemptId => new { attemptId });
			})
			.WithName("RetryQuiz")
			.WithSummary("Learner")
			.WithDescription("Creates a new empty attempt for an existing quiz. Returns raw attemptId.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
