using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.Quiz.Question;

public static class QuestionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"QUESTION.NOT_FOUND", "The question was not found.");

	public static readonly Error InvalidSavedItemId = Error.Validation(
		"QUESTION.INVALID_SAVED_ITEM_ID", "Saved item ID cannot be empty.");

	public static readonly Error InvalidTypeId = Error.Validation(
		"QUESTION.INVALID_TYPE_ID", "Question type ID cannot be empty.");

	public static readonly Error InvalidPosition = Error.Validation(
		"QUESTION.INVALID_POSITION", "Position must be greater than zero.");

	public static readonly Error EmptyPrompt = Error.Validation(
		"QUESTION.EMPTY_PROMPT", "Prompt cannot be empty.");

	public static readonly Error EmptyCorrectOption = Error.Validation(
		"QUESTION.EMPTY_CORRECT_OPTION", "Correct option ID cannot be empty.");
}
