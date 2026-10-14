using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetAllVideos;

internal sealed class GetAllVideosHandler : IRequestHandler<GetAllVideosQuery, Result<PaginatedList<VideoListDto>>>
{
	private readonly IVideoRepository _repository;

	public GetAllVideosHandler(IVideoRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<PaginatedList<VideoListDto>>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
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

		// Calculate total count before paging
		var totalCount = filteredVideos.Count();

		// Apply paging
		var items = filteredVideos
			.OrderByDescending(v => v.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(VideoListDto.FromEntity)
			.ToList();

		var pagedResult = PaginatedList<VideoListDto>.Create(items, totalCount, request.PageNumber, request.PageSize);

		return Result.Success(pagedResult);
	}
}
