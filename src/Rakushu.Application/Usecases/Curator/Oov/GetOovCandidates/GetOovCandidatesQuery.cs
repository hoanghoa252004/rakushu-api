using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;

public sealed record GetOovCandidatesQuery(
	int PageNumber = 1,
	int PageSize = 20,
	string? SearchTerm = null,
	string? Status = null
) : IRequest<Result<PaginatedList<OovCandidateDto>>>;
