using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking.DeleteVideoWatchHistory;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.DeleteVideoWatchHistory;

internal sealed class DeleteVideoWatchHistory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningVideoTrackingEndpoints()
			.MapDelete("/{videoId:guid}/watch-history", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteVideoWatchHistoryCommand(videoId), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteVideoWatchHistory")
			.WithSummary("Learner")
			.WithDescription("Deletes a single video's watch progress and encountered unit records from the learner's history.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
