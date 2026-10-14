using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Identity.User.Profile;

public sealed class ProfileId : StronglyTypedId<Guid>
{
	private ProfileId(Guid value) : base(value)
	{
	}

	public static ProfileId Create() => new(Guid.NewGuid());

	public static ProfileId From(Guid value) => new(value);
}
