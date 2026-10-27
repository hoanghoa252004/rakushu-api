using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.Quiz;

public static class QuizErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"QUIZ.NOT_FOUND", "The quiz was not found.");

	public static readonly Error InvalidUserId = Error.Validation(
		"QUIZ.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error NullQuestion = Error.Validation(
		"QUIZ.NULL_QUESTION", "Question cannot be null.");
}
