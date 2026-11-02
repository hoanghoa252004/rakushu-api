using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRole;

public sealed class ShadowingRoleId : StronglyTypedId<Guid>
{
	private ShadowingRoleId(Guid value) : base(value)
	{
	}

	public static ShadowingRoleId Create() => new(Guid.NewGuid());

	public static ShadowingRoleId From(Guid value) => new(value);
}