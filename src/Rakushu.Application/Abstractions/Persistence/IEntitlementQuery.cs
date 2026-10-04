using Rakushu.Application.Usecases.Subscription.Entitlement;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IEntitlementQuery
{
	Task<IReadOnlyCollection<EntitlementDto>> GetByPlanIdAsync(PlanId planId, CancellationToken cancellationToken = default);
}
