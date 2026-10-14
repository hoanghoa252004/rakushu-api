using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Series.GetAllSeries;

public sealed record GetAllSeriesQuery(
	int PageNumber = 1,
	int PageSize = 10) : IRequest<Result<PaginatedList<SeriesDto>>>;
