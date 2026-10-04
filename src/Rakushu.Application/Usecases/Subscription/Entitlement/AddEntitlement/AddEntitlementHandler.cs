using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.Entitlement;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.AddEntitlement;

internal sealed class AddEntitlementHandler : IRequestHandler<AddEntitlementCommand, Result<Guid>>
{
	private readonly IPlanRepository _planRepository;
	private readonly IFeatureRepository _featureRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public AddEntitlementHandler(
		IPlanRepository planRepository,
		IFeatureRepository featureRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_planRepository = planRepository;
		_featureRepository = featureRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(AddEntitlementCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Get Plan
			var plan = await _planRepository.GetByIdAsync(
				PlanId.From(request.PlanId),
				cancellationToken);

			if (plan is null)
			{
				return Result.Failure<Guid>(PlanErrors.NotFound);
			}

			// 2. Check Feature exists
			var feature = await _featureRepository.GetByIdAsync(
				FeatureId.From(request.FeatureId),
				cancellationToken);

			if (feature is null)
			{
				return Result.Failure<Guid>(FeatureErrors.NotFound);
			}

			if (feature.Status != FeatureStatus.Active)
			{
				return Result.Failure<Guid>(EntitlementErrors.FeatureNotActive);
			}

			// 3. Parse Enums
			if (!Enum.TryParse<LimitUnit>(request.LimitUnit, true, out var limitUnit))
			{
				return Result.Failure<Guid>(EntitlementErrors.InvalidLimitUnit);
			}

			if (!Enum.TryParse<LimitPeriod>(request.LimitPeriod, true, out var limitPeriod))
			{
				return Result.Failure<Guid>(EntitlementErrors.InvalidLimitPeriod);
			}

			var now = _systemClock.UtcNow;

			// 4. Add entitlement to plan aggregate
			var result = plan.AddEntitlement(
				feature.Id,
				request.IsEnabled,
				request.LimitValue,
				limitUnit,
				limitPeriod,
				now);

			if (result.IsFailure)
			{
				return Result.Failure<Guid>(result.Error);
			}

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
