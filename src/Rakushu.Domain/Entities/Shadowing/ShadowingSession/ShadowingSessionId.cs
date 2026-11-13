using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public sealed class ShadowingSessionId : StronglyTypedId<Guid>
{
	private ShadowingSessionId(Guid value) : base(value)
	{
	}

	public static ShadowingSessionId Create() => new(Guid.NewGuid());

	public static ShadowingSessionId From(Guid value) => new(value);
}