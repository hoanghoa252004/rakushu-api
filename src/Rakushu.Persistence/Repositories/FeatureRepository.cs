using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Repositories;

public sealed class FeatureRepository : BaseRepository<Feature, FeatureId>, IFeatureRepository
{
	public FeatureRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Feature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.Features
			.Include(f => f.Entitlements)
			.Include(f => f.SubscriptionUsages)
			.SingleOrDefaultAsync(f => f.Code == code, cancellationToken);
	}

	public override async Task<Feature?> GetByIdAsync(FeatureId id, CancellationToken cancellationToken = default)
	{
		return await _context.Features
			.Include(f => f.Entitlements)
			.Include(f => f.SubscriptionUsages)
			.SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
	}

	public override async Task<IEnumerable<Feature>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Features
			.Include(f => f.Entitlements)
			.Include(f => f.SubscriptionUsages)
			.OrderByDescending(f => f.UpdatedAt)
			.ToListAsync(cancellationToken);
	}
}
