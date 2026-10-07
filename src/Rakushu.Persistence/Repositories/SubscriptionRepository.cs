
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.User.Subscription;
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
			.ToListAsync(cancellationToken);
	}
}
