using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan.Entitlement;

public class EntitlementId : StronglyTypedId<Guid>
{
	private EntitlementId(Guid value) : base(value)
	{
	}

	public static EntitlementId Create() => new(Guid.NewGuid());

	public static EntitlementId From(Guid value) => new(value);
}
