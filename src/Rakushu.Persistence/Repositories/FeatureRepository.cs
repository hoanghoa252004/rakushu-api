using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Feature.ObjectValues;

namespace Rakushu.Persistence.Repositories;

public sealed class FeatureRepository : BaseRepository<Feature, FeatureId>, IFeatureRepository
{
	public FeatureRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Feature?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken = default)
	{
		return await _context.Features
			.Include(f => f.PlanEntitlements)
			.Include(f => f.SubscriptionUsages)
			.SingleOrDefaultAsync(f => f.Code == code, cancellationToken);
	}

	public override async Task<Feature?> GetByIdAsync(FeatureId id, CancellationToken cancellationToken = default)
	{
		return await _context.Features
			.Include(f => f.PlanEntitlements)
			.Include(f => f.SubscriptionUsages)
			.SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
	}
}
