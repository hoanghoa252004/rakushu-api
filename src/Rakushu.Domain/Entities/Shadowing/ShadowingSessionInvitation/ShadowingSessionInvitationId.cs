using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

public sealed class ShadowingSessionInvitationId : StronglyTypedId<Guid>
{
	private ShadowingSessionInvitationId(Guid value) : base(value)
	{
	}

	public static ShadowingSessionInvitationId Create() => new(Guid.NewGuid());

	public static ShadowingSessionInvitationId From(Guid value) => new(value);
}