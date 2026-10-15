using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Video.GetCuratedVideos;

public sealed record GetCuratedVideosQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchKey = null,
	Guid? ContentCategoryId = null,
	Guid? SeriesId = null
) : IRequest<Result<PaginatedList<VideoDetailDto>>>;
