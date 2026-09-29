using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Profile.Interest;

public sealed class InterestId : StronglyTypedId<Guid>
{
	private InterestId(Guid value) : base(value)
	{
	}

	public static InterestId Create() => new(Guid.NewGuid());

	public static InterestId From(Guid value) => new(value);
}
