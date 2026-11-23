using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.GetOovStats;

public sealed record GetOovStatsQuery(
	Guid? VideoId = null,
	string? Status = null,
	string? Search = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<PaginatedList<CuratorOovStatDto>>>;
