using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Subscription.Entitlement;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;

internal sealed class GetEntitlementsHandler : IRequestHandler<GetEntitlementsQuery, Result<IReadOnlyCollection<EntitlementDto>>>
{
	private readonly IPlanRepository _planRepository;
	private readonly IEntitlementQuery _entitlementQuery;

	public GetEntitlementsHandler(
		IPlanRepository planRepository,
		IEntitlementQuery entitlementQuery)
	{
		_planRepository = planRepository;
		_entitlementQuery = entitlementQuery;
	}

	public async Task<Result<IReadOnlyCollection<EntitlementDto>>> Handle(GetEntitlementsQuery request, CancellationToken cancellationToken)
	{
		var plan = await _planRepository.GetByIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		if (plan is null)
		{
			return Result.Failure<IReadOnlyCollection<EntitlementDto>>(PlanErrors.NotFound);
		}

		var items = await _entitlementQuery.GetByPlanIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		return Result.Success(items);
	}
}
