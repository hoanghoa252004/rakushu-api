
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Subscriptions.Plan;
using Rakushu.Domain.Entities.Subscriptions.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class SubscriptionRepository : BaseRepository<Subscription, SubscriptionId>, ISubscriptionRepository
{
	public SubscriptionRepository(RakushuDbContext context) : base(context) { }

	public override async Task<IEnumerable<Subscription>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Subscriptions
			.Include(s => s.User)
			.Include(s => s.Payment)
			.Include(s => s.SubscriptionUsages)
			.Include(s => s.Plan)
				.ThenInclude(p => p.Entitlements)
					.ThenInclude(p => p.Feature)
			.ToListAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<Subscription> Items, int TotalCount)> GetPagedByUserAsync(
		UserId userId,
		PlanId? planId,
		SubscriptionStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Subscriptions
			.AsNoTracking()
			.Where(s => s.UserId == userId);

		if (planId is not null)
			query = query.Where(s => s.PlanId == planId);

		if (status.HasValue)
			query = query.Where(s => s.Status == status.Value);

		var total = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(s => s.StartAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Include(s => s.User)
			.Include(s => s.Payment)
			.Include(s => s.SubscriptionUsages)
				.ThenInclude(su => su.Feature)
			.Include(s => s.Plan)
				.ThenInclude(p => p.Entitlements)
					.ThenInclude(e => e.Feature)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public override async Task<Subscription?> GetByIdAsync(SubscriptionId id, CancellationToken cancellationToken = default)
	{
		return await _context.Subscriptions
			.Include(s => s.User)
			.Include(s => s.Payment)
			.Include(s => s.SubscriptionUsages)
			.Include(s => s.Plan)
				.ThenInclude(p => p.Entitlements)
					.ThenInclude(p => p.Feature)
			.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
	}
}
