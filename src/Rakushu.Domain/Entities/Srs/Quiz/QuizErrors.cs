using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the Quiz aggregate.
/// </summary>
public static class QuizErrors
{
	public static readonly Error NotFound = Error.NotFound("SRS.QUIZ_NOT_FOUND", "The quiz was not found.");
	public static readonly Error InvalidUserId = Error.Validation("SRS.INVALID_USER_ID", "A valid user ID is required.");
	public static readonly Error InvalidQuestion = Error.Validation("SRS.INVALID_QUESTION", "A valid question is required.");
	public static readonly Error InvalidQuestionPosition = Error.Validation("SRS.INVALID_QUESTION_POSITION", "Position must be greater than 0.");
	public static readonly Error InvalidPrompt = Error.Validation("SRS.INVALID_QUIZ_PROMPT", "A question prompt is required.");
	public static readonly Error InvalidCorrectOptionId = Error.Validation("SRS.INVALID_CORRECT_OPTION_ID", "A correct option ID is required.");
}
