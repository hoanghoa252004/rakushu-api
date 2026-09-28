using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.OovCandidate;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class OovCandidateRepository : BaseRepository<OovCandidate, OovCandidateId>, IOovCandidateRepository
{
	public OovCandidateRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<OovCandidate?> GetByTermAsync(string term, CancellationToken cancellationToken = default)
	{
		return await _context.OovCandidates
			.Include(o => o.CuratorReview)
			.FirstOrDefaultAsync(o => o.Term == term, cancellationToken);
	}

	public async Task<OovCandidate?> GetWithReviewAsync(OovCandidateId id, CancellationToken cancellationToken = default)
	{
		return await _context.OovCandidates
			.Include(o => o.CuratorReview)
			.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
	}
}
