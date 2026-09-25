using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;

namespace Rakushu.Application.Usecases.PlanEntitlement.UpdatePlanEntitlement;

internal sealed class UpdatePlanEntitlementHandler : IRequestHandler<UpdatePlanEntitlementCommand, Result>
{
	private readonly IPlanRepository _planRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public UpdatePlanEntitlementHandler(
		IPlanRepository planRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdatePlanEntitlementCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Get Plan
			var plan = await _planRepository.GetByIdAsync(
				PlanId.From(request.PlanId),
				cancellationToken);

			if (plan is null)
			{
				return Result.Failure(PlanErrors.NotFound);
			}

			// 2. Parse Enums
			if (!Enum.TryParse<LimitUnit>(request.LimitUnit, true, out var limitUnit))
			{
				return Result.Failure(PlanEntitlementErrors.InvalidLimitUnit);
			}

			if (!Enum.TryParse<LimitPeriod>(request.LimitPeriod, true, out var limitPeriod))
			{
				return Result.Failure(PlanEntitlementErrors.InvalidLimitPeriod);
			}

			var now = _systemClock.UtcNow;

			// 3. Update entitlement on plan aggregate
			var result = plan.UpdateEntitlement(
				PlanEntitlementId.From(request.EntitlementId),
				request.IsEnabled,
				request.LimitValue,
				limitUnit,
				limitPeriod,
				now);

			if (result.IsFailure)
			{
				return result;
			}

			return Result.Success();
		}, cancellationToken);
	}
}
