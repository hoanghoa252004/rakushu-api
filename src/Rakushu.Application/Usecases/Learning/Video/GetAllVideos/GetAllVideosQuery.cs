using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Video.GetAllVideos;

public sealed record GetAllVideosQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchKey = null,
	Guid? ContentCategoryId = null,
	Guid? SeriesId = null,
	Guid? UserId = null
) : IRequest<Result<PaginatedList<VideoListDto>>>;
