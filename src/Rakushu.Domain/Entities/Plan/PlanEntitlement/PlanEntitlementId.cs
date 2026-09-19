using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan.PlanEntitlement;

public class PlanEntitlementId : StronglyTypedId<Guid>
{
	private PlanEntitlementId(Guid value) : base(value)
	{
	}

	public static PlanEntitlementId Create() => new(Guid.NewGuid());

	public static PlanEntitlementId From(Guid value) => new(value);
}
