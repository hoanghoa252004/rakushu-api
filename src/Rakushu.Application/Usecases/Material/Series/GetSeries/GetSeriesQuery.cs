using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.GetSeries;

public sealed record GetSeriesQuery(
	int PageNumber = 1,
	int PageSize = 10) : IRequest<Result<PaginatedList<SeriesDto>>>;
