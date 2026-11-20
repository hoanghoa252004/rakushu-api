using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;

public sealed record RecordWatchHeartbeatCommand(
	Guid VideoId,
	double CurrentPositionSeconds,
	WatchedRangeInput? WatchedRange,
	double PlaybackRate = 1.0,
	double? VideoDurationSeconds = null) : IRequest<Result<RecordWatchHeartbeatResponseDto>>;
