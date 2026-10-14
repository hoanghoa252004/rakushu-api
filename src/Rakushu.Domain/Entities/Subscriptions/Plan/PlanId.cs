using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Subscriptions.Plan;

public class PlanId : StronglyTypedId<Guid>
{
	private PlanId(Guid value) : base(value)
	{
	}

	public static PlanId Create() => new(Guid.NewGuid());

	public static PlanId From(Guid value) => new(value);
}