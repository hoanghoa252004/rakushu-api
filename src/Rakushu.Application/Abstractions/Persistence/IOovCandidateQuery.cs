using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;
using Rakushu.Domain.Entities.OovCandidate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IOovCandidateQuery
{
	Task<OovCandidateDetailDto?> GetByIdAsync(OovCandidateId id, CancellationToken cancellationToken = default);
	Task<(IReadOnlyCollection<OovCandidateDto> Items, int TotalCount)> GetOovCandidatesAsync(GetOovCandidatesQuery query, CancellationToken cancellationToken = default);
}
