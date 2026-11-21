using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Video;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.GetCuratedVideos;

internal sealed class GetCuratedVideosHandler : IRequestHandler<GetCuratedVideosQuery, Result<PaginatedList<VideoDetailDto>>>
{
	private readonly IVideoRepository _repository;
	private readonly IStorageService _storageService;

	public GetCuratedVideosHandler(IVideoRepository repository, IStorageService storageService)
	{
		_repository = repository;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<VideoDetailDto>>> Handle(GetCuratedVideosQuery request, CancellationToken cancellationToken)
	{
		var (videos, totalCount) = await _repository.GetPagedAsync(
			new[] { VideoSource.Curated },
			VideoStatus.Ready,
			null,
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
