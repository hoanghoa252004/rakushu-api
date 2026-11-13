using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Quiz.CreateQuiz;

internal sealed class CreateQuiz : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/", async (
				[FromBody] CreateQuizRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateQuizAttemptCommand(dto.ItemIds, dto.Mode, dto.DiscardActive);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetQuizAttempt", r => new
				{
					attemptId = r.AttemptId,
					skippedItems = r.SkippedItems
				});
			})
			.WithName("CreateQuiz")
			.WithSummary("Learner")
			.WithDescription("Creates a new quiz and an associated initial quiz attempt with 0 answered details. Returns raw attemptId.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateQuizRequestDto(
	IReadOnlyList<Guid> ItemIds,
	string? Mode = null,
	bool DiscardActive = false);
