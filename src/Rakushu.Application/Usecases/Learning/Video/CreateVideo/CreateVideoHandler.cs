using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.CreateVideo;

internal sealed class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<Guid>>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;
	private readonly ICurrentUserContext _currentUserContext;

	public CreateVideoHandler(
		IVideoRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<Guid>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Verify current user exists
			if (_currentUserContext.UserId is null)
				return Result.Failure<Guid>(VideoErrors.UnauthorizedVideoSourceForCurator);

			// Only Curator can create Curated videos
			if (request.SourceType == VideoSource.Curated && 
				_currentUserContext.Role != RoleCodes.LinguisticCurator)
			{
				return Result.Failure<Guid>(VideoErrors.UnauthorizedVideoSourceForCurator);
			}

			var now = _systemClock.UtcNow;

			// Determine initial status based on source type
			var initialStatus = request.SourceType == VideoSource.Curated 
				? VideoStatus.Processing 
				: VideoStatus.Pending;

			var videoResult = Domain.Entities.Video.Video.Create(
				request.Slug,
				request.Title,
				request.Duration,
				ContentCategoryId.From(request.ContentCategoryId),
				request.SeriesId.HasValue ? SeriesId.From(request.SeriesId.Value) : null,
				request.SortOrder,
				request.SourceType,
				initialStatus,
				UserId.From(_currentUserContext.UserId.Value),
				now,
				now,
				request.Description,
				request.SourceUrl);

			if (videoResult.IsFailure)
				return Result.Failure<Guid>(videoResult.Error);

			_repository.Add(videoResult.Value);
			return Result.Success(videoResult.Value.Id.Value);
		}, cancellationToken);
	}
}
