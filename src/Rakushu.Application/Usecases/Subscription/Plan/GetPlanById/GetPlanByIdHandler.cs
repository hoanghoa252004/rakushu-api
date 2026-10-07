using MediatR;
using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

public sealed class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanDto>>
{
	// DAOs
	private readonly IPlanRepository _planRepository;

	public GetPlanByIdHandler(IPlanRepository planRepository)
	{
		_planRepository = planRepository;
	}

	public async Task<Result<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
	{
		var plan = await _planRepository.GetByIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		if (plan == null)
		{
			return Result.Failure<PlanDto>(PlanErrors.NotFound);
		}

		var planDto = new PlanDto(
			plan.Id.Value,
			plan.Code,
			plan.Name,
			plan.JapaneseName,
			plan.Price,
			plan.Currency.ToString(),
			plan.BillingCycle.ToString(),
			plan.IsActive,
			plan.CreatedAt,
			plan.UpdatedAt,
			plan.Entitlements.Select(e => new EntitlementDto(
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
					e.Feature.Description))).ToList(),
			plan.Description
		);

		return Result.Success(planDto);
	}
}
