using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.UpdateAttemptDetail;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.UpdateAttemptDetail;

internal sealed class UpdateAttemptDetail : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsAttemptEndpoints()
			.MapPut("/{attemptId:guid}/details/{detailId:guid}", async (
				[FromRoute] Guid attemptId,
				[FromRoute] Guid detailId,
				[FromBody] UpdateAttemptDetailRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateAttemptDetailCommand(
					attemptId,
					detailId,
					dto.SelectedAnswer,
					dto.TimeSpentMs);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateAttemptDetail")
			.WithSummary("Learner")
			.WithDescription("Updates an existing answer detail before final submission.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateAttemptDetailRequestDto(
	string? SelectedAnswer,
	int TimeSpentMs);
