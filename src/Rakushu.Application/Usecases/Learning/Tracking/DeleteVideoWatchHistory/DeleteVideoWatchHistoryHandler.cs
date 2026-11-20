using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Tracking.DeleteVideoWatchHistory;

internal sealed class DeleteVideoWatchHistoryHandler : IRequestHandler<DeleteVideoWatchHistoryCommand, Result>
{
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteVideoWatchHistoryHandler(
		ILearnerVideoProgressRepository progressRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ICurrentUserContext currentUserContext,
		IUnitOfWork unitOfWork)
	{
		_progressRepository = progressRepository;
		_encounterRepository = encounterRepository;
		_currentUserContext = currentUserContext;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteVideoWatchHistoryCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure(UserErrors.NotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var videoId = VideoId.From(request.VideoId);
			await _progressRepository.DeleteByUserAndVideoAsync(userId, videoId, cancellationToken);
			await _encounterRepository.DeleteByUserAndVideoAsync(userId, videoId, cancellationToken);
			return Result.Success();
		}, cancellationToken);
	}
}
