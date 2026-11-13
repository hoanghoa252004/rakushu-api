using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public static class ShadowingSessionErrors
{
	public static readonly Error Unauthenticated = Error.Unauthorized(
		"SHADOWING_SESSION.UNAUTHENTICATED",
		"The current user could not be identified.");

	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_SESSION.NOT_FOUND",
		"The shadowing session was not found.");

	public static readonly Error InvalidTitle = Error.Validation(
		"SHADOWING_SESSION.INVALID_TITLE",
		"Session title is required and must not exceed 200 characters.");

	public static readonly Error InvalidVisibility = Error.Validation(
		"SHADOWING_SESSION.INVALID_VISIBILITY",
		"The session visibility is invalid.");

	public static readonly Error StartTimeTooSoon = Error.Validation(
		"SHADOWING_SESSION.START_TIME_TOO_SOON",
		"The scheduled start time must be at least 10 minutes in the future.");

	public static readonly Error NotScheduled = Error.Conflict(
		"SHADOWING_SESSION.NOT_SCHEDULED",
		"The operation is only allowed while the session is in Scheduled status.");

	public static readonly Error NotHost = Error.Forbidden(
		"SHADOWING_SESSION.NOT_HOST",
		"Only the host of the session can perform this action.");

	public static readonly Error MaterialNotFound = Error.NotFound(
		"SHADOWING_SESSION.MATERIAL_NOT_FOUND",
		"The shadowing material was not found.");

	public static readonly Error MaterialNotReady = Error.Conflict(
		"SHADOWING_SESSION.MATERIAL_NOT_READY",
		"The shadowing material is not ready.");

	public static readonly Error MaterialNotAllowed = Error.Forbidden(
		"SHADOWING_SESSION.MATERIAL_NOT_ALLOWED",
		"You can only create a session from your own video's material or a curated video's material.");

	public static readonly Error ActiveSessionExists = Error.Conflict(
		"SHADOWING_SESSION.ACTIVE_SESSION_EXISTS",
		"You already host a session that is not completed yet. Finish it before creating a new one.");

	public static readonly Error ScheduleConflict = Error.Conflict(
		"SHADOWING_SESSION.SCHEDULE_CONFLICT",
		"The session time overlaps with another session you participate in.");

	public static readonly Error SessionFull = Error.Validation(
		"SHADOWING_SESSION.FULL",
		"The session already has as many members as the material has roles.");

	public static readonly Error HasMembers = Error.Conflict(
		"SHADOWING_SESSION.HAS_MEMBERS",
		"The session already has members other than the host.");

	public static readonly Error NotInProgress = Error.Conflict(
		"SHADOWING_SESSION.NOT_IN_PROGRESS",
		"The operation is only allowed while the session is in InProgress status.");

	public static readonly Error MembersIncomplete = Error.Validation(
		"SHADOWING_SESSION.MEMBERS_INCOMPLETE",
		"The session does not have as many members as the material has roles.");

	public static readonly Error RolesNotAssigned = Error.Validation(
		"SHADOWING_SESSION.ROLES_NOT_ASSIGNED",
		"Every member must choose a role before the session can start.");

	public static readonly Error NotMember = Error.Forbidden(
		"SHADOWING_SESSION.NOT_MEMBER",
		"You are not a member of this session.");

	public static readonly Error NotPublic = Error.Forbidden(
		"SHADOWING_SESSION.NOT_PUBLIC",
		"The session is not public.");
}
