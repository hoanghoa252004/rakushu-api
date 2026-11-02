using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

public sealed class ShadowingSessionInvitation : Entity<ShadowingSessionInvitationId>
{
	public ShadowingSessionId ShadowingSessionId { get; private set; } = null!;
	public UserId HostId { get; private set; } = null!;
	public UserId LearnerId { get; private set; } = null!;
	public SessionInvitationStatus Status { get; private set; }
	public DateTimeOffset SentAt { get; private set; }
	public DateTimeOffset ExpiredAt { get; private set; }
	public DateTimeOffset? ResponsedAt { get; private set; }
}
