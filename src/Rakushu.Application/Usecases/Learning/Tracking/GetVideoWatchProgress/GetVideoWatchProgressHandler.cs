using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVideoWatchProgress;

internal sealed class GetVideoWatchProgressHandler
	: IRequestHandler<GetVideoWatchProgressQuery, Result<VideoWatchProgressDto>>
{
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetVideoWatchProgressHandler(
		ILearnerVideoProgressRepository progressRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ICurrentUserContext currentUserContext)
	{
		_progressRepository = progressRepository;
		_encounterRepository = encounterRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<VideoWatchProgressDto>> Handle(
		GetVideoWatchProgressQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<VideoWatchProgressDto>(UserErrors.NotFound);
		}

		var videoId = VideoId.From(request.VideoId);
		var progress = await _progressRepository.GetByUserAndVideoAsync(userId, videoId, cancellationToken);
		var encountered = await _encounterRepository.GetByUserAndVideoAsync(userId, videoId, cancellationToken);
		var encounteredUnitIds = encountered
			.Select(x => x.LearningUnitId.Value)
			.Distinct()
			.ToList();

		if (progress is null)
		{
			return Result.Success(new VideoWatchProgressDto(
				request.VideoId,
				LastWatchedSecond: 0,
				TotalWatchedSeconds: 0,
				CompletionRate: 0,
				IsCompleted: false,
				WatchedIntervals: [],
				EncounteredLearningUnitIds: encounteredUnitIds));
		}

		return Result.Success(new VideoWatchProgressDto(
			request.VideoId,
			progress.LastWatchedSecond,
			progress.TotalWatchedSeconds,
			progress.CompletionRate,
			progress.IsCompleted,
			progress.GetWatchedIntervals(),
			encounteredUnitIds));
	}
}
