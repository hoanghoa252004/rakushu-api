using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

public static class ShadowingSessionInvitationErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_SESSION_INVITATION.NOT_FOUND",
		"The invitation was not found.");

	public static readonly Error LearnerNotFound = Error.NotFound(
		"SHADOWING_SESSION_INVITATION.LEARNER_NOT_FOUND",
		"One or more invited learners were not found.");

	public static readonly Error CannotInviteHost = Error.Validation(
		"SHADOWING_SESSION_INVITATION.CANNOT_INVITE_HOST",
		"The host cannot invite themselves.");

	public static readonly Error AlreadyMember = Error.Conflict(
		"SHADOWING_SESSION_INVITATION.ALREADY_MEMBER",
		"One or more invited learners are already members of the session.");

	public static readonly Error AlreadyInvited = Error.Conflict(
		"SHADOWING_SESSION_INVITATION.ALREADY_INVITED",
		"One or more learners already have a pending invitation for this session.");

	public static readonly Error InvalidExpiry = Error.Validation(
		"SHADOWING_SESSION_INVITATION.INVALID_EXPIRY",
		"It is too late to invite learners to this session.");

	public static readonly Error NotPending = Error.Conflict(
		"SHADOWING_SESSION_INVITATION.NOT_PENDING",
		"The invitation is no longer pending.");

	public static readonly Error Expired = Error.Conflict(
		"SHADOWING_SESSION_INVITATION.EXPIRED",
		"The invitation has expired.");

	public static readonly Error InvalidResponse = Error.Validation(
		"SHADOWING_SESSION_INVITATION.INVALID_RESPONSE",
		"The invitation can only be responded with Accepted or Rejected.");

	public static readonly Error NotHost = Error.Forbidden(
		"SHADOWING_SESSION_INVITATION.NOT_HOST",
		"Only the host can manage this invitation.");
}
