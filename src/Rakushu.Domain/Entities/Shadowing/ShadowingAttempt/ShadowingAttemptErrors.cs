using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

public static class ShadowingAttemptErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_ATTEMPT.NOT_FOUND",
		"The shadowing attempt was not found.");

	public static readonly Error NotOwner = Error.Forbidden(
		"SHADOWING_ATTEMPT.NOT_OWNER",
		"The attempt does not belong to the current learner.");

	public static readonly Error NotInProgress = Error.Conflict(
		"SHADOWING_ATTEMPT.NOT_IN_PROGRESS",
		"The operation is only allowed while the attempt is in InProgress status.");

	public static readonly Error SessionNotInProgress = Error.Conflict(
		"SHADOWING_ATTEMPT.SESSION_NOT_IN_PROGRESS",
		"The operation is only allowed while the session is in InProgress status.");

	public static readonly Error RoleLineNotFound = Error.NotFound(
		"SHADOWING_ATTEMPT.ROLE_LINE_NOT_FOUND",
		"The role line was not found.");

	public static readonly Error RoleLineNotYours = Error.Forbidden(
		"SHADOWING_ATTEMPT.ROLE_LINE_NOT_YOURS",
		"The role line does not belong to the role you chose.");

	public static readonly Error PreviousLineNotCompleted = Error.Conflict(
		"SHADOWING_ATTEMPT.PREVIOUS_LINE_NOT_COMPLETED",
		"You cannot do this line yet because the previous role line has not been completed.");

	public static readonly Error DetailNotFound = Error.NotFound(
		"SHADOWING_ATTEMPT.DETAIL_NOT_FOUND",
		"The attempt detail was not found.");

	public static readonly Error DetailExists = Error.Conflict(
		"SHADOWING_ATTEMPT.DETAIL_EXISTS",
		"This role line already has an attempt detail.");

	public static readonly Error InvalidAudio = Error.Validation(
		"SHADOWING_ATTEMPT.INVALID_AUDIO",
		"Audio key is required (max 500 characters) and duration must be positive.");

	public static readonly Error HasNoDetails = Error.Conflict(
		"SHADOWING_ATTEMPT.INCOMPLETE",
		"All your role lines must be completed before submitting.");
}
