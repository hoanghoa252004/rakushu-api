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

	private ShadowingSessionInvitation() { }

	private ShadowingSessionInvitation(
		ShadowingSessionId sessionId,
		UserId hostId,
		UserId learnerId,
		DateTimeOffset sentAt,
		DateTimeOffset expiredAt) : base(ShadowingSessionInvitationId.Create())
	{
		ShadowingSessionId = sessionId;
		HostId = hostId;
		LearnerId = learnerId;
		Status = SessionInvitationStatus.Pending;
		SentAt = sentAt;
		ExpiredAt = expiredAt;
	}

	public static ShadowingSessionInvitation Create(
		ShadowingSessionId sessionId,
		UserId hostId,
		UserId learnerId,
		DateTimeOffset sentAt,
		DateTimeOffset expiredAt)
		=> new(sessionId, hostId, learnerId, sentAt, expiredAt);

	public Domain.Common.Results.Result Respond(SessionInvitationStatus newStatus, DateTimeOffset now)
	{
		if (newStatus != SessionInvitationStatus.Accepted && newStatus != SessionInvitationStatus.Rejected)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.InvalidResponse);
		if (Status != SessionInvitationStatus.Pending)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.NotPending);
		if (ExpiredAt <= now)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.Expired);

		Status = newStatus;
		ResponsedAt = now;
		return Domain.Common.Results.Result.Success();
	}

	public Domain.Common.Results.Result EnsureCanRespond(DateTimeOffset now)
	{
		if (Status != SessionInvitationStatus.Pending)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.NotPending);
		if (ExpiredAt <= now)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.Expired);
		return Domain.Common.Results.Result.Success();
	}

	public Domain.Common.Results.Result ChangeLearner(UserId learnerId, DateTimeOffset now)
	{
		if (Status != SessionInvitationStatus.Pending)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.NotPending);
		if (ExpiredAt <= now)
			return Domain.Common.Results.Result.Failure(ShadowingSessionInvitationErrors.Expired);

		LearnerId = learnerId;
		return Domain.Common.Results.Result.Success();
	}

	public void Revoke(DateTimeOffset now)
	{
		if (Status != SessionInvitationStatus.Pending) return;
		Status = SessionInvitationStatus.Revoked;
		ResponsedAt = now;
	}

	public void UpdateExpiry(DateTimeOffset expiredAt, DateTimeOffset now)
	{
		if (Status != SessionInvitationStatus.Pending) return;
		ExpiredAt = expiredAt;
		if (expiredAt <= now) MarkExpired(now);
	}

	public void MarkExpired(DateTimeOffset now)
	{
		if (Status != SessionInvitationStatus.Pending) return;
		Status = SessionInvitationStatus.Expired;
		ResponsedAt = now;
	}
}
