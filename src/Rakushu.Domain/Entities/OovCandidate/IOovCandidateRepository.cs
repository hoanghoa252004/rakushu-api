using Rakushu.Domain.Common.Contract;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.OovCandidate;

public interface IOovCandidateRepository : IBaseRepository<OovCandidate, OovCandidateId>
{
	Task<OovCandidate?> GetByTermAsync(string term, CancellationToken cancellationToken = default);
	Task<OovCandidate?> GetWithReviewAsync(OovCandidateId id, CancellationToken cancellationToken = default);
}
