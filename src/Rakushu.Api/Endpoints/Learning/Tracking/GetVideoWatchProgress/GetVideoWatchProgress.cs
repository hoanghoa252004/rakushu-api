using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.GetVideoWatchProgress;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.GetVideoWatchProgress;

internal sealed class GetVideoWatchProgress : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningVideoTrackingEndpoints()
			.MapGet("/{videoId:guid}/watch-progress", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetVideoWatchProgressQuery(videoId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetVideoWatchProgress")
			.WithSummary("Learner")
			.WithDescription("Retrieves the learner's watch progress, last watched position for resume, merged watched intervals, and encountered learning unit IDs for a specific video.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<VideoWatchProgressDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
