using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.QuestionType;

public static class QuestionTypeErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"QUESTION_TYPE.NOT_FOUND", "The question type was not found.");

	public static readonly Error EmptyCode = Error.Validation(
		"QUESTION_TYPE.EMPTY_CODE", "Question type code cannot be empty.");

	public static readonly Error EmptyName = Error.Validation(
		"QUESTION_TYPE.EMPTY_NAME", "Question type name cannot be empty.");
}
