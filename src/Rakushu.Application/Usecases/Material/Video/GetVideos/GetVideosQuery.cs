using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Video.GetVideos;

public sealed record GetVideosQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchKey = null,
	Guid? ContentCategoryId = null,
	Guid? SeriesId = null,
	Guid? UserId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<VideoDetailDto>>>;
