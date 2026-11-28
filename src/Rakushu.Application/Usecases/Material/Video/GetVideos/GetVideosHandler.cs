using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.GetVideos;

internal sealed class GetVideosHandler : IRequestHandler<GetVideosQuery, Result<PaginatedList<VideoDetailDto>>>
{
	private readonly IVideoRepository _repository;
	private readonly IStorageService _storageService;

	public GetVideosHandler(IVideoRepository repository, IStorageService storageService)
	{
		_repository = repository;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<VideoDetailDto>>> Handle(GetVideosQuery request, CancellationToken cancellationToken)
	{
		VideoStatus? status = null;
		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			if (!Enum.TryParse<VideoStatus>(request.Status, true, out var parsedStatus))
				return Result.Failure<PaginatedList<VideoDetailDto>>(VideoErrors.InvalidStatus);
			status = parsedStatus;
		}

		var (videos, totalCount) = await _repository.GetPagedAsync(
			Enum.GetValues<VideoSource>(),
			status,
			request.UserId,
			request.SearchKey,
			request.ContentCategoryId,
			request.SeriesId,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var items = videos.Select(VideoDetailDto.FromEntity).ToList();

		// Generate pre-signed URLs for media assets
		var enrichedItems = await Task.WhenAll(
			items.Select(async video =>
			{
				var mediaAssets = await Task.WhenAll(
					video.MediaAssets.Select(async mediaAsset =>
					{
						var signedUrl = await _storageService
							.CreatePresignedReadUrlAsync(
								mediaAsset.StorageKey,
								cancellationToken);

						return mediaAsset with
						{
							StorageKey = signedUrl
						};
					}));

				if (video.Series?.ThumbnailKey != null)
				{
					var thumbnailUrl = await _storageService.CreatePresignedReadUrlAsync(video.Series.ThumbnailKey, cancellationToken);

					video = video with { Series = video.Series with { ThumbnailKey = thumbnailUrl } };
				}

				return video with
				{
					MediaAssets = mediaAssets
				};
			}));

		// Create paginated result
		var pagedResult = PaginatedList<VideoDetailDto>.Create(
			enrichedItems,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(pagedResult);
	}
}
