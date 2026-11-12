using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

public sealed class ShadowingAttemptDetailId : StronglyTypedId<Guid>
{
	private ShadowingAttemptDetailId(Guid value) : base(value)
	{
	}

	public static ShadowingAttemptDetailId Create() => new(Guid.NewGuid());

	public static ShadowingAttemptDetailId From(Guid value) => new(value);
}