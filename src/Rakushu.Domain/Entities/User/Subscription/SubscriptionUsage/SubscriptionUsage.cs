using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;

public sealed class SubscriptionUsage : Entity<SubscriptionUsageId>
{
	public SubscriptionId SubscriptionId { get; private set; } = null!;
	public FeatureId FeatureId { get; private set; } = null!;
	public DateTimeOffset PeriodStart { get; private set; }
	public DateTimeOffset PeriodEnd { get; private set; }
	public int UsedValue { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

}
