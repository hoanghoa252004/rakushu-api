using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Plan.ObjectValues;

namespace Rakushu.Domain.Entities.Plan;

public interface IPlanRepository : IBaseRepository<Plan, PlanId>
{
	Task<Plan?> GetByCodeAsync(PlanCode code, CancellationToken cancellationToken = default);
}
