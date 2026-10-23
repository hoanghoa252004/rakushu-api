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
		var videos = await _repository.GetAllAsync(cancellationToken);

		var filteredVideos = videos.AsEnumerable();

		// Filter by SearchKey (title or slug)
		if (!string.IsNullOrWhiteSpace(request.SearchKey))
		{
			var searchKeyLower = request.SearchKey.ToLower();
			filteredVideos = filteredVideos.Where(v =>
				v.Title.ToLower().Contains(searchKeyLower) ||
				v.Slug.ToLower().Contains(searchKeyLower));
		}

		// Filter by ContentCategoryId
		if (request.ContentCategoryId.HasValue)
		{
			var categoryId = ContentCategoryId.From(request.ContentCategoryId.Value);
			filteredVideos = filteredVideos.Where(v => v.ContentCategoryId == categoryId);
		}

		// Filter by SeriesId
		if (request.SeriesId.HasValue)
		{
			var seriesId = SeriesId.From(request.SeriesId.Value);
			filteredVideos = filteredVideos.Where(v => v.SeriesId == seriesId);
		}

		// Filter by UserId
		if (request.UserId.HasValue)
		{
			var userId = UserId.From(request.UserId.Value);
			filteredVideos = filteredVideos.Where(v => v.LearnerId == userId);
		}

		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<VideoStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<VideoDetailDto>>(VideoErrors.InvalidStatus);
		}

		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			filteredVideos = filteredVideos.Where(v => v.Status.ToString() == request.Status.ToString());
		}
		
		// Calculate total count before paging
		var totalCount = filteredVideos.Count();

		// Apply paging
		var items = filteredVideos
			.OrderByDescending(v => v.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(VideoDetailDto.FromEntity)
			.ToList();

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
