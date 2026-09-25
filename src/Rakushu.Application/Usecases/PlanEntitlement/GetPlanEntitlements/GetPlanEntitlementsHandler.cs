using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.PlanEntitlement.GetPlanEntitlements;

internal sealed class GetPlanEntitlementsHandler : IRequestHandler<GetPlanEntitlementsQuery, Result<IReadOnlyCollection<PlanEntitlementDto>>>
{
	private readonly IPlanRepository _planRepository;
	private readonly IPlanEntitlementQuery _planEntitlementQuery;

	public GetPlanEntitlementsHandler(
		IPlanRepository planRepository,
		IPlanEntitlementQuery planEntitlementQuery)
	{
		_planRepository = planRepository;
		_planEntitlementQuery = planEntitlementQuery;
	}

	public async Task<Result<IReadOnlyCollection<PlanEntitlementDto>>> Handle(GetPlanEntitlementsQuery request, CancellationToken cancellationToken)
	{
		var plan = await _planRepository.GetByIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		if (plan is null)
		{
			return Result.Failure<IReadOnlyCollection<PlanEntitlementDto>>(PlanErrors.NotFound);
		}

		var items = await _planEntitlementQuery.GetByPlanIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		return Result.Success(items);
	}
}
