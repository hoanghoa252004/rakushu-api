using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription;

public sealed class Subscription : Entity<SubscriptionId>
{
	public UserId UserId { get; private set; } = null!;
	public PlanId PlanId { get; private set; } = null!;
	public SubscriptionStatus Status { get; private set; }
	public DateTimeOffset StartDate { get; private set; }
}
