using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateAttemptDetail;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.CreateAttemptDetail;

internal sealed class CreateAttemptDetail : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsAttemptEndpoints()
			.MapPost("/{attemptId:guid}/details", async (
				[FromRoute] Guid attemptId,
				[FromBody] CreateAttemptDetailRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateAttemptDetailCommand(
					attemptId,
					dto.QuizItemId,
					dto.SelectedAnswer,
					dto.TimeSpentMs);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetQuizAttempt", _ => new { attemptId });
			})
			.WithName("CreateAttemptDetail")
			.WithSummary("Learner")
			.WithDescription("Records an answer for a quiz question during an active attempt. Returns raw detailId.")
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

internal sealed record CreateAttemptDetailRequestDto(
	Guid QuizItemId,
	string? SelectedAnswer,
	int TimeSpentMs);
