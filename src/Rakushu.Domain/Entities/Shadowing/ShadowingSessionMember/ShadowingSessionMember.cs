using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

public sealed class ShadowingSessionMember : Entity<ShadowingSessionMemberId>
{
	public ShadowingSessionId ShadowingSessionId { get; private set; } = null!;
	public ShadowingRoleId? ShadowingRoleId { get; private set; }
	public UserId MemberId { get; private set; } = null!;
	public SessionMemberJoinType JoinType { get; private set; }
	public ShadowingSessionInvitationId? ShadowingSessionInvitationId { get; private set; }
	public DateTimeOffset JoinedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// ShadowingSession
	public ShadowingSession.ShadowingSession ShadowingSession { get; private set; } = null!;

	// ShadowingRole
	public ShadowingRole.ShadowingRole? ShadowingRole { get; private set; }

	// Member
	public User Member { get; private set; } = null!;

	// ShadowingSessionInvitation
	public ShadowingSessionInvitation.ShadowingSessionInvitation? ShadowingSessionInvitation { get; private set; }
	private readonly List<ShadowingAttempt.ShadowingAttempt> _attempts = new();

	public IReadOnlyCollection<ShadowingAttempt.ShadowingAttempt> Attempts =>
		_attempts.AsReadOnly();

	private ShadowingSessionMember() { }

	private ShadowingSessionMember(
		ShadowingSessionId sessionId,
		UserId memberId,
		SessionMemberJoinType joinType,
		ShadowingSessionInvitationId? invitationId,
		DateTimeOffset joinedAt) : base(ShadowingSessionMemberId.Create())
	{
		ShadowingSessionId = sessionId;
		MemberId = memberId;
		JoinType = joinType;
		ShadowingSessionInvitationId = invitationId;
		JoinedAt = joinedAt;
	}

	public static ShadowingSessionMember CreateHost(ShadowingSessionId sessionId, UserId hostId, DateTimeOffset now)
		=> new(sessionId, hostId, SessionMemberJoinType.Host, null, now);

	public static ShadowingSessionMember CreateFromInvitation(
		ShadowingSessionId sessionId, UserId memberId, ShadowingSessionInvitationId invitationId, DateTimeOffset now)
		=> new(sessionId, memberId, SessionMemberJoinType.Invitation, invitationId, now);

	public static ShadowingSessionMember CreatePublicJoin(ShadowingSessionId sessionId, UserId memberId, DateTimeOffset now)
		=> new(sessionId, memberId, SessionMemberJoinType.PublicJoin, null, now);

	public void ChooseRole(ShadowingRoleId roleId) => ShadowingRoleId = roleId;
}
