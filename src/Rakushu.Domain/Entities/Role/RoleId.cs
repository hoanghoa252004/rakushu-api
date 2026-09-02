using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Role;

public class RoleId : StronglyTypedId<Guid>
{
	public RoleId(Guid value) : base(value)
	{
	}

	public static RoleId Create() => new(Guid.NewGuid());

	public static RoleId From(Guid value) => new(value);

}
