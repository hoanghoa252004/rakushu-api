using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Persistence.Repositories;

public sealed class PlanRepository : BaseRepository<Plan, PlanId>, IPlanRepository
{
	public PlanRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Plan?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.Plans
			.Include(p => p.Entitlements)
				.ThenInclude(e => e.Feature)
			.Include(p => p.Subscriptions)
			.SingleOrDefaultAsync(p => p.Code == code, cancellationToken);
	}

	public override async Task<Plan?> GetByIdAsync(PlanId id, CancellationToken cancellationToken = default)
	{
		return await _context.Plans
			.Include(p => p.Entitlements)
				.ThenInclude(e => e.Feature)
			.Include(p => p.Subscriptions)
			.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public override async Task<IEnumerable<Plan>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Plans
			.Include(f => f.Entitlements)
				.ThenInclude(e => e.Feature)
			.Include(f => f.Subscriptions)
			.OrderByDescending(f => f.UpdatedAt)
			.ToListAsync(cancellationToken);
	}
}
