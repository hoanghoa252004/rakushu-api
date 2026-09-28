using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Domain.Common.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;

internal sealed class GetOovCandidatesHandler : IRequestHandler<GetOovCandidatesQuery, Result<PaginatedList<OovCandidateDto>>>
{
	private readonly IOovCandidateQuery _query;

	public GetOovCandidatesHandler(IOovCandidateQuery query)
	{
		_query = query;
	}

	public async Task<Result<PaginatedList<OovCandidateDto>>> Handle(
		GetOovCandidatesQuery request,
		CancellationToken cancellationToken)
	{
		var (items, totalCount) = await _query.GetOovCandidatesAsync(request, cancellationToken);

		var paginated = new PaginatedList<OovCandidateDto>(
			items,
			request.PageNumber,
			request.PageSize,
			totalCount
		);

		return Result<PaginatedList<OovCandidateDto>>.Success(paginated);
	}
}
