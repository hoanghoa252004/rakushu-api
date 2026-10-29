using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/quiz-attempts", async (
				[FromBody] CreateQuizAttemptRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateQuizAttemptCommand(dto.ItemIds);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetSrsQuizAttemptResult", resp => new { attemptId = resp.AttemptId });
			})
			.WithName("CreateSrsQuizAttempt")
			.WithSummary("Generate an SRS quiz session from selected vocabulary items")
			.WithDescription("Creates an SRS quiz attempt sourced directly from curated bank questions for learner-selected vocabulary items.")
			.RequireAuthorization()
			.Produces<QuizAttemptStartedResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateQuizAttemptRequestDto(
	IReadOnlyList<Guid> ItemIds);
