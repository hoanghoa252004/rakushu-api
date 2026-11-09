using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt;

public static class QuizAttemptErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"QUIZ_ATTEMPT.NOT_FOUND", "The quiz attempt was not found.");

	public static readonly Error InvalidUserId = Error.Validation(
		"QUIZ_ATTEMPT.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error InvalidQuizId = Error.Validation(
		"QUIZ_ATTEMPT.INVALID_QUIZ_ID", "Quiz ID cannot be empty.");

	public static readonly Error NotInProgress = Error.Validation(
		"QUIZ_ATTEMPT.NOT_IN_PROGRESS", "The quiz attempt is not currently in progress.");

	public static readonly Error AlreadyCompleted = Error.Conflict(
		"QUIZ_ATTEMPT.ALREADY_COMPLETED", "The quiz attempt has already been completed.");

	public static readonly Error AlreadyCanceled = Error.Conflict(
		"QUIZ_ATTEMPT.ALREADY_CANCELED", "The quiz attempt has already been canceled.");

	public static readonly Error NotCompleted = Error.Validation(
		"QUIZ_ATTEMPT.NOT_COMPLETED", "The quiz attempt is not completed yet.");

	public static readonly Error AttemptExpired = Error.Validation(
		"QUIZ_ATTEMPT.EXPIRED", "The quiz attempt has expired.");

	public static readonly Error ActiveAttemptInProgress = Error.Conflict(
		"QUIZ_ATTEMPT.ACTIVE_ATTEMPT_EXISTS", "You already have an active quiz attempt in progress. Please complete or delete it before starting a new one.");

	public static readonly Error NoItemsAnswered = Error.Validation(
		"QUIZ_ATTEMPT.NO_ITEMS_ANSWERED", "At least one question must be answered before submitting the quiz attempt.");

	public static readonly Error CannotDeleteAttemptWithAnswers = Error.Validation(
		"QUIZ_ATTEMPT.CANNOT_DELETE_WITH_ANSWERS", "Cannot delete a quiz attempt that already has answered questions. Please submit it instead.");

	public static readonly Error QuestionAlreadyAnswered = Error.Conflict(
		"QUIZ_ATTEMPT.QUESTION_ALREADY_ANSWERED", "This question has already been answered in this attempt. Use PUT to update your answer.");
}
