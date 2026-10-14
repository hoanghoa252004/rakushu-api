using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Subscriptions.Subscription;

public class SubscriptionId : StronglyTypedId<Guid>
{
	public SubscriptionId(Guid value) : base(value)
	{
	}

	public static SubscriptionId Create() => new(Guid.NewGuid());

	public static SubscriptionId From(Guid value) => new(value);

}