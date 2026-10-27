using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public static class BankQuestionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"BANK_QUESTION.NOT_FOUND", "The bank question was not found.");

	public static readonly Error InvalidMeaning = Error.Validation(
		"BANK_QUESTION.INVALID_MEANING", "Knowledge meaning ID cannot be empty.");

	public static readonly Error InvalidType = Error.Validation(
		"BANK_QUESTION.INVALID_TYPE", "Question type ID cannot be empty.");

	public static readonly Error EmptyPrompt = Error.Validation(
		"BANK_QUESTION.EMPTY_PROMPT", "Question prompt cannot be empty.");

	public static readonly Error EmptyCorrectChoice = Error.Validation(
		"BANK_QUESTION.EMPTY_CORRECT_CHOICE", "Correct choice ID cannot be empty.");
}
