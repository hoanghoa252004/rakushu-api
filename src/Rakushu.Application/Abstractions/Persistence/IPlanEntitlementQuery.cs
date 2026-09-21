using Rakushu.Application.Usecases.PlanEntitlement;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IPlanEntitlementQuery
{
	Task<IReadOnlyCollection<PlanEntitlementDto>> GetByPlanIdAsync(PlanId planId, CancellationToken cancellationToken = default);
}
