using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public static class SavedItemErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SAVED_ITEM.NOT_FOUND", "The saved item was not found.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"SAVED_ITEM.ALREADY_EXISTS", "This vocabulary meaning is already saved in your notebook.");

	public static readonly Error CannotActivateRevokedMeaning = Error.Conflict(
		"SAVED_ITEM.CANNOT_ACTIVATE_REVOKED", "This vocabulary item has been revoked by curators and cannot be activated for review.");

	public static readonly Error InvalidUserId = Error.Validation(
		"SAVED_ITEM.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error InvalidMeaningId = Error.Validation(
		"SAVED_ITEM.INVALID_MEANING_ID", "Knowledge meaning ID cannot be empty.");

	public static readonly Error InvalidTranscriptSegmentId = Error.Validation(
		"SAVED_ITEM.INVALID_TRANSCRIPT_SEGMENT_ID", "Transcript segment ID cannot be empty.");

	public static readonly Error EmptyExpression = Error.Validation(
		"SAVED_ITEM.EMPTY_EXPRESSION", "Expression cannot be empty.");

	public static readonly Error EmptyMeaning = Error.Validation(
		"SAVED_ITEM.EMPTY_MEANING", "Meaning cannot be empty.");
}
