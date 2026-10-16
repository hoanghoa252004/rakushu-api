using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.Video.CreateVideo;

internal sealed class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<Guid>>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public CreateVideoHandler(
		IVideoRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<Guid>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Validate the source type
			if (!string.IsNullOrWhiteSpace(request.SourceType) && !Enum.TryParse<VideoSource>(request.SourceType, true, out _))
			{
				return Result.Failure<Guid>(VideoErrors.InvalidSourceType);
			}

			var sourceType = Enum.Parse<VideoSource>(request.SourceType, true);

			// Verify current user exists
			if (_currentUserContext.UserId is null)
				return Result.Failure<Guid>(VideoErrors.UnauthorizedVideoSourceForCurator);

			// Only Curator can create Curated videos
			if (sourceType == VideoSource.Curated && _currentUserContext.Role != RoleCodes.LinguisticCurator)
				return Result.Failure<Guid>(VideoErrors.UnauthorizedVideoSourceForCurator);

			var now = _systemClock.UtcNow;

			// Determine initial status based on source type
			var initialStatus = sourceType == VideoSource.Curated ? VideoStatus.Draft : VideoStatus.Pending;


			var videoResult = Domain.Entities.Material.Video.Video.Create(
				request.Slug,
				request.Title,
				request.Duration,
				ContentCategoryId.From(request.ContentCategoryId),
				request.SeriesId.HasValue ? SeriesId.From(request.SeriesId.Value) : null,
				request.SortOrder,
				sourceType,
				initialStatus,
				UserId.From(_currentUserContext.UserId.Value),
				now,
				now,
				request.Description,
				request.SourceUrl);

			if (videoResult.IsFailure)
				return Result.Failure<Guid>(videoResult.Error);

			// Get metadata of media asset from storage service
			var videoMetadataResult = await _storageService.GetMediaAssetMetadataAsync(request.StorageKeyOfVideo, cancellationToken);

			var thumbnailMetadataResult = await _storageService.GetMediaAssetMetadataAsync(request.StorageKeyOfThumbnail, cancellationToken);

			// Create media assets for video and thumbnail
			var videoMediaAsset = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset.Create(
				videoResult.Value.Id,
				MediaAssetType.Video,
				request.StorageKeyOfVideo,
				videoMetadataResult.ContentType,
				videoMetadataResult.FileName,
				videoMetadataResult.FileSizeBytes);

			var thumbnailMediaAsset = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset.Create(
				videoResult.Value.Id,
				MediaAssetType.Thumbnail,
				request.StorageKeyOfThumbnail,
				thumbnailMetadataResult.ContentType,
				thumbnailMetadataResult.FileName,
				thumbnailMetadataResult.FileSizeBytes);

			videoResult.Value.AddMediaAsset(videoMediaAsset.Value);

			videoResult.Value.AddMediaAsset(thumbnailMediaAsset.Value);

			_repository.Add(videoResult.Value);
			return Result.Success(videoResult.Value.Id.Value);
		}, cancellationToken);
	}
}
