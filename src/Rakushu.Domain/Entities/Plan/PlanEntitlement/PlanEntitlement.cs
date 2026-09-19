using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.User.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		bool isEnable,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		) : base(planEntitlementId)
	{
		PlanId = planId;
		FeatureId = featureId;
		IsEnabled = isEnable;
		LimitValue = limitValue;
		LimitUnit = limitUnit;
		LimitPeriod = limitPeriod;
	}

	public static Result<PlanEntitlement> Create(
		PlanId planId,
		FeatureId featureId,
		bool isEnable,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod
		)
	{
		var planEntitlement = new PlanEntitlement(
			PlanEntitlementId.Create(),
			planId,
			featureId,
			isEnable,
			limitValue,
			limitUnit,
			limitPeriod
			);

		return Result.Success(planEntitlement);
	}
}
