using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.GetAllSeries;

public sealed record GetSeriesQuery(
	int PageNumber = 1,
	int PageSize = 10) : IRequest<Result<PaginatedList<SeriesDto>>>;
