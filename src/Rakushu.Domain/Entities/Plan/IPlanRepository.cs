using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Plan;

public interface IPlanRepository : IBaseRepository<Plan, PlanId>
{
	Task<Plan?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
