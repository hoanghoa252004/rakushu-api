using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User;

public class UserId : StronglyTypedId<Guid>
{
	private UserId(Guid value) : base(value)
	{
	}

	public static UserId Create() => new(Guid.NewGuid());

	public static UserId From(Guid value) => new(value);
}
