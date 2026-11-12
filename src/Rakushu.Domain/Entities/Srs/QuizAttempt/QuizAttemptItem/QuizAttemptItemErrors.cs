using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

public static class QuizAttemptItemErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"QUIZ_ATTEMPT_ITEM.NOT_FOUND", "The quiz attempt item was not found.");

	public static readonly Error InvalidQuestionId = Error.Validation(
		"QUIZ_ATTEMPT_ITEM.INVALID_QUESTION_ID", "Question ID cannot be empty.");

	public static readonly Error InvalidPosition = Error.Validation(
		"QUIZ_ATTEMPT_ITEM.INVALID_POSITION", "Position must be greater than zero.");
}
