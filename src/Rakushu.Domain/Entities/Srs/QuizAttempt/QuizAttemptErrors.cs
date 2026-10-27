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
}
