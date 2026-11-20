using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVideoWatchProgress;

public sealed record GetVideoWatchProgressQuery(
	Guid VideoId) : IRequest<Result<VideoWatchProgressDto>>;
