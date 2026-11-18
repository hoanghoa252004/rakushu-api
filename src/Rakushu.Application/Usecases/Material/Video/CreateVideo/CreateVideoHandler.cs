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
	private static readonly System.Text.RegularExpressions.Regex YoutubeUrlRegex = new(
		@"^(https?://)?((www\.|m\.|music\.)?youtube\.com/(watch\?(.*&)?v=|shorts/|embed/|live/)|youtu\.be/)[A-Za-z0-9_-]{11}([?&#/].*)?$",
		System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

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

			var isExternalImport = sourceType == VideoSource.ImportedFromExternal;

			if (isExternalImport)
			{
				if (string.IsNullOrWhiteSpace(request.SourceUrl))
					return Result.Failure<Guid>(VideoErrors.SourceUrlRequired);

				if (!YoutubeUrlRegex.IsMatch(request.SourceUrl.Trim()))
					return Result.Failure<Guid>(VideoErrors.InvalidYoutubeUrl);
			}
			else if (string.IsNullOrWhiteSpace(request.StorageKeyOfVideo))
			{
				return Result.Failure<Guid>(VideoErrors.StorageKeyOfVideoRequired);
			}

			var now = _systemClock.UtcNow;

			// Determine initial status based on source type
			var initialStatus = sourceType switch
			{
				VideoSource.Curated => VideoStatus.Draft,
				VideoSource.ImportedFromExternal => VideoStatus.Importing,
				_ => VideoStatus.Moderating
			};


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
				request.SourceUrl?.Trim());

			if (videoResult.IsFailure)
				return Result.Failure<Guid>(videoResult.Error);

			var thumbnailMetadataResult = await _storageService.GetMediaAssetMetadataAsync(request.StorageKeyOfThumbnail, cancellationToken);

			// Create media assets for video (not for external import, created by import worker) and thumbnail
			var thumbnailMediaAsset = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset.Create(
				videoResult.Value.Id,
				MediaAssetType.Thumbnail,
				request.StorageKeyOfThumbnail,
				thumbnailMetadataResult.ContentType,
				thumbnailMetadataResult.FileName,
				thumbnailMetadataResult.FileSizeBytes);

			if (!isExternalImport)
			{
				var videoMetadataResult = await _storageService.GetMediaAssetMetadataAsync(request.StorageKeyOfVideo!, cancellationToken);

				var videoMediaAsset = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset.Create(
					videoResult.Value.Id,
					MediaAssetType.Video,
					request.StorageKeyOfVideo!,
					videoMetadataResult.ContentType,
					videoMetadataResult.FileName,
					videoMetadataResult.FileSizeBytes);

				videoResult.Value.AddMediaAsset(videoMediaAsset.Value);
			}

			videoResult.Value.AddMediaAsset(thumbnailMediaAsset.Value);
			videoResult.Value.AddMediaAsset(thumbnailMediaAsset.Value);

			_repository.Add(videoResult.Value);
			return Result.Success(videoResult.Value.Id.Value);
		}, cancellationToken);
	}
}
