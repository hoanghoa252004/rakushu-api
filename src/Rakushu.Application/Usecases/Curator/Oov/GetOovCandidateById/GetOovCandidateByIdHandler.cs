using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.OovCandidate;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Rakushu.Application.Abstractions.Persistence.Queries;

namespace Rakushu.Application.Usecases.Curator.Oov.GetOovCandidateById;

internal sealed class GetOovCandidateByIdHandler : IRequestHandler<GetOovCandidateByIdQuery, Result<OovCandidateDetailDto>>
{
	private readonly IOovCandidateQuery _query;

	public GetOovCandidateByIdHandler(IOovCandidateQuery query)
	{
		_query = query;
	}

	public async Task<Result<OovCandidateDetailDto>> Handle(
		GetOovCandidateByIdQuery request,
		CancellationToken cancellationToken)
	{
		var result = await _query.GetByIdAsync(OovCandidateId.From(request.Id), cancellationToken);
		if (result is null)
		{
			return Result.Failure<OovCandidateDetailDto>(OovCandidateErrors.NotFound);
		}

		return Result.Success(result);
	}
}
