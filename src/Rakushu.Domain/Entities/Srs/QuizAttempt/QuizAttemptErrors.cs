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

	public static readonly Error NotCompleted = Error.Validation(
		"QUIZ_ATTEMPT.NOT_COMPLETED", "Only completed quiz attempts can be retried.");

	public static readonly Error NoMistakesToRetry = Error.Validation(
		"QUIZ_ATTEMPT.NO_MISTAKES", "There are no mistake questions in this quiz attempt to retry.");

	public static readonly Error AttemptExpired = Error.Validation(
		"QUIZ_ATTEMPT.EXPIRED", "The quiz attempt has exceeded the 1-hour time limit and has been abandoned.");

	public static readonly Error ActiveAttemptInProgress = Error.Conflict(
		"QUIZ_ATTEMPT.ACTIVE_ATTEMPT_EXISTS", "You already have an active quiz attempt in progress. Please complete or cancel it before starting a new one.");
}
