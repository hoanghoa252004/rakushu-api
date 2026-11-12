using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public sealed class ShadowingRoleLineId : StronglyTypedId<Guid>
{
	private ShadowingRoleLineId(Guid value) : base(value)
	{
	}

	public static ShadowingRoleLineId Create() => new(Guid.NewGuid());

	public static ShadowingRoleLineId From(Guid value) => new(value);
}