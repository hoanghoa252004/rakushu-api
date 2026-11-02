using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

public enum SessionInvitationStatus
{
	Pending = 1,
	Accepted = 2,
	Rejected = 3,
	Expired = 4,
	Revoked = 5
}
