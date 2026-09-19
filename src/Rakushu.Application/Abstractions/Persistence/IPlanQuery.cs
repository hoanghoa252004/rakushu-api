using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Application.Usecases.Plan.GetPlans;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IPlanQuery
{
	Task<PlanDto?> GetByIdAsync(PlanId id, CancellationToken cancellationToken = default);
	Task<(IReadOnlyCollection<PlanDto> Items, int TotalCount)> GetPlansAsync(GetPlansQuery query, CancellationToken cancellationToken = default);

}
