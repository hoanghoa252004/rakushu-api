using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

public sealed class ShadowingAttemptId : StronglyTypedId<Guid>
{
	private ShadowingAttemptId(Guid value) : base(value)
	{
	}

	public static ShadowingAttemptId Create() => new(Guid.NewGuid());

	public static ShadowingAttemptId From(Guid value) => new(value);
}