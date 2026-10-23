using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.DeleteVideo;

internal sealed class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUserContext _currentUserContext;

	public DeleteVideoHandler(
		IVideoRepository repository,
		IUnitOfWork unitOfWork,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var currentUserId = _currentUserContext.UserId;
			var currentUserRole = _currentUserContext.Role;

			if (currentUserRole == RoleCodes.Learner)
			{
				// Learner can only delete their own videos if status is Pending
				if (video.LearnerId != UserId.From(currentUserId!.Value))
					return Result.Failure(VideoErrors.CannotDeleteVideo);

				if (video.Status != VideoStatus.Pending)
					return Result.Failure(VideoErrors.CannotDeleteVideo);
			}
			else if (currentUserRole == RoleCodes.LinguisticCurator)
			{
				// Curator can only delete Curated videos with Draft status
				if (video.SourceType != VideoSource.Curated)
					return Result.Failure(VideoErrors.CannotDeleteVideo);

				if (video.Status != VideoStatus.Draft)
					return Result.Failure(VideoErrors.CannotDeleteVideo);
			}
			else
			{
				// Admin or other roles cannot delete
				return Result.Failure(VideoErrors.CannotDeleteVideo);
			}

			_repository.Delete(video);
			return Result.Success();
		}, cancellationToken);
	}
}
