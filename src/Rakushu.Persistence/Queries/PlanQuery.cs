using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Application.Usecases.Plan.GetPlans;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using System.Collections.ObjectModel;

namespace Rakushu.Persistence.Queries;

public sealed class PlanQuery : IPlanQuery
{
	private readonly RakushuDbContext _dbContext;

	public PlanQuery(RakushuDbContext context)
	{
		_dbContext = context;
	}

	public async Task<PlanDto?> GetByIdAsync(PlanId id, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Plans
			.AsNoTracking()
			.Where(p => p.Id == id)
			.Select(p => new PlanDto(
				p.Id.Value,
				p.Code.Value,
				p.Name,
				p.Price,
				p.Currency.ToString(),
				p.BillingCycle.ToString(),
				p.Status.ToString(),
				p.CreatedAt,
				p.UpdatedAt,
				p.Description))
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<(IReadOnlyCollection<PlanDto> Items, int TotalCount)> GetPlansAsync(GetPlansQuery query, CancellationToken cancellationToken = default)
	{
		IQueryable<Plan> plans = _dbContext.Plans.AsNoTracking();

		// Filter by status
		if (query.Status != null)
		{
			plans = plans.Where(u => u.Status == query.Status);
		}

		// Filter by Features
		if (query.FeatureIds != null && query.FeatureIds.Any() == true)
		{
			//Collection<FeatureId> featIds = new();
			//foreach (var featureId in query.FeatureIds)
			//{
			//	featIds.Add(FeatureId.From(featureId));
			//}
			//plans = plans.Where(
			//	p => featIds.All(
			//		id => p.PlanEntitlements.Any(
			//			pe => pe.FeatureId == id)));
		}

		// Count BEFORE pagination
		var totalCount = await plans
			.CountAsync(cancellationToken);

		// Pagination + Projection
		var items = await plans
			.OrderBy(u => u.Id)
			.Skip((query.PageNumber - 1) * query.PageSize)
			.Take(query.PageSize)
			.Select(p => new PlanDto(
				p.Id.Value,
				p.Code.Value,
				p.Name,
				p.Price,
				p.Currency.ToString(),
				p.BillingCycle.ToString(),
				p.Status.ToString(),
				p.CreatedAt,
				p.UpdatedAt,
				p.Description))
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}
}
