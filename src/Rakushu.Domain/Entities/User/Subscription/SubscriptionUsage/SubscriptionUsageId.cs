using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;

public class SubscriptionUsageId : StronglyTypedId<Guid>
{
	private SubscriptionUsageId(Guid value) : base(value)
	{
	}

	public static SubscriptionUsageId Create() => new(Guid.NewGuid());

	public static SubscriptionUsageId From(Guid value) => new(value);
}