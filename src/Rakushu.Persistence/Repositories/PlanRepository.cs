using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.ObjectValues;
using Rakushu.Persistence.Repositories;

namespace Rakushu.Persistence.Repositories;

public sealed class PlanRepository : BaseRepository<Plan, PlanId>, IPlanRepository
{
	public PlanRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Plan?> GetByCodeAsync(PlanCode code, CancellationToken cancellationToken = default)
	{
		return await _context.Plans
			.Include(p => p.PlanEntitlements)
			.Include(p => p.Subscriptions)
			.SingleOrDefaultAsync(p => p.Code == code, cancellationToken);
	}

	public override async Task<Plan?> GetByIdAsync(PlanId id, CancellationToken cancellationToken = default)
	{
		return await _context.Plans
			.Include(p => p.PlanEntitlements)
			.Include(p => p.Subscriptions)
			.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
	}
}
