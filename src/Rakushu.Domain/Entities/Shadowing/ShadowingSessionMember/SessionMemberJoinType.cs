using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

public enum SessionMemberJoinType
{
	Host = 1,
	Invitation = 2,
	PublicJoin = 3
}