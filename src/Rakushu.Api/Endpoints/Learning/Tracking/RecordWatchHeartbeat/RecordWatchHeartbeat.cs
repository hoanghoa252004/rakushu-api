using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.RecordWatchHeartbeat;

internal sealed class RecordWatchHeartbeat : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningVideoTrackingEndpoints()
			.MapPost("/{videoId:guid}/watch-heartbeat", async (
				[FromRoute] Guid videoId,
				[FromBody] RecordWatchHeartbeatRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new RecordWatchHeartbeatCommand(
					videoId,
					dto.CurrentPositionSeconds,
					dto.WatchedRange,
					dto.PlaybackRate ?? 1.0,
					dto.VideoDurationSeconds);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("RecordWatchHeartbeat")
			.WithSummary("Learner")
			.WithDescription("Records a video playback interval heartbeat, merges watched intervals, records encountered learning units with anti-massed-repetition capping, and adjusts SRS DueAt for frequently encountered vocabulary.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<RecordWatchHeartbeatResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record RecordWatchHeartbeatRequestDto(
	double CurrentPositionSeconds,
	WatchedRangeInput? WatchedRange,
	double? PlaybackRate = 1.0,
	double? VideoDurationSeconds = null);
