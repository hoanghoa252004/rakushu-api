using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Domain.Entities.Plan.PlanEntitlement;

public sealed class PlanEntitlement : Entity<PlanEntitlementId>
{
	public PlanId PlanId { get; private set; } = null!;
	public FeatureId FeatureId { get; private set; } = null!;
	public bool IsEnabled { get; private set; }
	public int LimitValue { get; private set; }
	public LimitUnit LimitUnit { get; private set; }
	public LimitPeriod LimitPeriod { get; private set; }

	// NAVIGATION PROPERTIES
	// Plan:
	public Plan Plan { get; private set; } = null!;

	// Feature:
	public Feature.Feature Feature { get; private set; } = null!;

	private PlanEntitlement() { }

	private PlanEntitlement(
		PlanEntitlementId planEntitlementId,
		PlanId planId,
		FeatureId featureId,
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		) : base(planEntitlementId)
	{
		PlanId = planId;
		FeatureId = featureId;
		IsEnabled = isEnabled;
		LimitValue = limitValue;
		LimitUnit = limitUnit;
		LimitPeriod = limitPeriod;
	}

	public static Result<PlanEntitlement> Create(
		PlanId planId,
		FeatureId featureId,
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		)
	{
		if (limitValue < 0)
		{
			return Result.Failure<PlanEntitlement>(PlanEntitlementErrors.InvalidLimitValue);
		}

		if (!Enum.IsDefined(limitUnit))
		{
			return Result.Failure<PlanEntitlement>(PlanEntitlementErrors.InvalidLimitUnit);
		}

		if (!Enum.IsDefined(limitPeriod))
		{
			return Result.Failure<PlanEntitlement>(PlanEntitlementErrors.InvalidLimitPeriod);
		}

		var planEntitlement = new PlanEntitlement(
			PlanEntitlementId.Create(),
			planId,
			featureId,
			isEnabled,
			limitValue,
			limitUnit,
			limitPeriod
			);

		return Result.Success(planEntitlement);
	}

	public Result Update(
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		)
	{
		if (limitValue < 0)
		{
			return Result.Failure(PlanEntitlementErrors.InvalidLimitValue);
		}

		if (!Enum.IsDefined(limitUnit))
		{
			return Result.Failure(PlanEntitlementErrors.InvalidLimitUnit);
		}

		if (!Enum.IsDefined(limitPeriod))
		{
			return Result.Failure(PlanEntitlementErrors.InvalidLimitPeriod);
		}

		IsEnabled = isEnabled;
		LimitValue = limitValue;
		LimitUnit = limitUnit;
		LimitPeriod = limitPeriod;

		return Result.Success();
	}
}
