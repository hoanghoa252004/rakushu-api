using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public static class FlashcardSessionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"FLASHCARD_SESSION.NOT_FOUND",
		"Flashcard session not found.");

	public static readonly Error InvalidUserId = Error.Validation(
		"FLASHCARD_SESSION.INVALID_USER_ID",
		"User id is required.");

	public static readonly Error EmptyItems = Error.Validation(
		"FLASHCARD_SESSION.EMPTY_ITEMS",
		"Flashcard session must contain at least one item.");

	public static readonly Error NotInProgress = Error.Validation(
		"FLASHCARD_SESSION.NOT_IN_PROGRESS",
		"Flashcard session is not in progress.");

	public static readonly Error Expired = Error.Validation(
		"FLASHCARD_SESSION.EXPIRED",
		"Flashcard session has expired and can no longer be interacted with.");

	public static readonly Error NotCompleted = Error.Validation(
		"FLASHCARD_SESSION.NOT_COMPLETED",
		"Flashcard session is not completed yet.");

	public static readonly Error InvalidRating = Error.Validation(
		"FLASHCARD_SESSION.INVALID_RATING",
		"Rating must be 'Forgot', 'Hard', or 'Easy'.");

	public static readonly Error IncompleteRatings = Error.Validation(
		"FLASHCARD_SESSION.INCOMPLETE_RATINGS",
		"All flashcard items must be rated before completing the session.");

	public static readonly Error NoItemsReviewed = Error.Validation(
		"FLASHCARD_SESSION.NO_ITEMS_REVIEWED",
		"At least one flashcard item must be reviewed before completing the session.");

	public static readonly Error ItemNotFound = Error.NotFound(
		"FLASHCARD_SESSION.ITEM_NOT_FOUND",
		"One or more flashcard items do not belong to this session.");

	public static readonly Error ActiveSessionInProgress = Error.Conflict(
		"FLASHCARD_SESSION.ACTIVE_SESSION_EXISTS",
		"You already have an active flashcard session in progress. Please complete or cancel it before starting a new one.");
}
