using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetMyVideos;

internal sealed class GetMyVideosHandler : IRequestHandler<GetMyVideosQuery, Result<PaginatedList<VideoListDto>>>
{
	private readonly IVideoRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetMyVideosHandler(
		IVideoRepository repository,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<PaginatedList<VideoListDto>>> Handle(GetMyVideosQuery request, CancellationToken cancellationToken)
	{
		if (_currentUserContext.UserId is null)
			return Result.Failure<PaginatedList<VideoListDto>>(VideoErrors.NotFound);

		var userId = UserId.From(_currentUserContext.UserId.Value);
		var videos = await _repository.GetAllAsync(cancellationToken);

		// Filter videos created by current user with UploadedByLearner or ImportedFromExternal source
		var filteredVideos = videos.Where(v =>
			v.CreatedBy == userId &&
			(v.SourceType == VideoSource.UploadedByLearner ||
			 v.SourceType == VideoSource.ImportedFromExternal));

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
