using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Subscription;

public class Plan : AggregateRoot<PlanId>
{
	public string Name { get; private set; } = null!;
	public string Description { get; private set;  } = null!;
	public Decimal Price { get; private set; }
	public BillingCycle BillingCycle { get; private set; }

}
