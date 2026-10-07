using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlans;

internal sealed class GetPlansHandler : IRequestHandler<GetPlansQuery, Result<IReadOnlyCollection<PlanDto>>>
{
	// DAOs
	private readonly IPlanRepository _planRepository;

	// Context
	private readonly ICurrentUserContext _currentUserContext;

	public GetPlansHandler(
		IPlanRepository planRepository,
		ICurrentUserContext currentUserContext)
	{
		_planRepository = planRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<IReadOnlyCollection<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
	{
		var plans = await _planRepository.GetAllAsync(cancellationToken);

		// If not admin, only return active plans
		var isAdmin = _currentUserContext.Role == RoleCodes.SystemAdministrator;

		if (!isAdmin)
		{
			plans = plans.Where(p => p.IsActive).ToList();
		}

		var planDtos = plans.Select(plan => new PlanDto(
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
		)).ToList();

		return Result.Success<IReadOnlyCollection<PlanDto>>(planDtos);
	}
}
