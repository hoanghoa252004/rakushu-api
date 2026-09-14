using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Subscription;
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
}
