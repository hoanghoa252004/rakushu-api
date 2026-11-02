using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;


public sealed class ShadowingSessionMemberId : StronglyTypedId<Guid>
{
	private ShadowingSessionMemberId(Guid value) : base(value)
	{
	}

	public static ShadowingSessionMemberId Create() => new(Guid.NewGuid());

	public static ShadowingSessionMemberId From(Guid value) => new(value);
}