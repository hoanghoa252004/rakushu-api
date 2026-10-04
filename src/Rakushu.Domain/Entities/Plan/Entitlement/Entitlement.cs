using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Domain.Entities.Plan.Entitlement;

public sealed class Entitlement : Entity<EntitlementId>
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

	private Entitlement() { }

	private Entitlement(
		EntitlementId entitlementId,
		PlanId planId,
		FeatureId featureId,
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		) : base(entitlementId)
	{
		PlanId = planId;
		FeatureId = featureId;
		IsEnabled = isEnabled;
		LimitValue = limitValue;
		LimitUnit = limitUnit;
		LimitPeriod = limitPeriod;
	}

	public static Result<Entitlement> Create(
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
			return Result.Failure<Entitlement>(EntitlementErrors.InvalidLimitValue);
		}

		if (!Enum.IsDefined(limitUnit))
		{
			return Result.Failure<Entitlement>(EntitlementErrors.InvalidLimitUnit);
		}

		if (!Enum.IsDefined(limitPeriod))
		{
			return Result.Failure<Entitlement>(EntitlementErrors.InvalidLimitPeriod);
		}

		var entitlement = new Entitlement(
			EntitlementId.Create(),
			planId,
			featureId,
			isEnabled,
			limitValue,
			limitUnit,
			limitPeriod
			);

		return Result.Success(entitlement);
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
			return Result.Failure(EntitlementErrors.InvalidLimitValue);
		}

		if (!Enum.IsDefined(limitUnit))
		{
			return Result.Failure(EntitlementErrors.InvalidLimitUnit);
		}

		if (!Enum.IsDefined(limitPeriod))
		{
			return Result.Failure(EntitlementErrors.InvalidLimitPeriod);
		}

		IsEnabled = isEnabled;
		LimitValue = limitValue;
		LimitUnit = limitUnit;
		LimitPeriod = limitPeriod;

		return Result.Success();
	}
}
