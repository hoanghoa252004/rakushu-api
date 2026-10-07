using MediatR;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;

internal sealed class GetEntitlementsHandler : IRequestHandler<GetEntitlementsQuery, Result<IReadOnlyCollection<EntitlementDto>>>
{
	private readonly IPlanRepository _planRepository;

	public GetEntitlementsHandler(
		IPlanRepository planRepository)
	{
		_planRepository = planRepository;
	}

	public async Task<Result<IReadOnlyCollection<EntitlementDto>>> Handle(GetEntitlementsQuery request, CancellationToken cancellationToken)
	{
		var planId = PlanId.From(request.PlanId);

		var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);

		if (plan is null)
		{
			return Result.Failure<IReadOnlyCollection<EntitlementDto>>(PlanErrors.NotFound);
		}

		var items = plan.Entitlements.Select(e => new EntitlementDto(
			e.Id.Value,
			e.IsEnabled,
			e.LimitUnit.ToString(),
			e.LimitValue,
			e.LimitPeriod.ToString(),
			new FeatureDto(
				e.Feature.Id.Value,
				e.Feature.Code,
				e.Feature.Name,
				e.Feature.IsActive,
				e.Feature.CreatedAt,
				e.Feature.UpdatedAt,
				e.Feature.Description
			)
		)).ToList();

		return Result.Success<IReadOnlyCollection<EntitlementDto>>(items);
	}
}
