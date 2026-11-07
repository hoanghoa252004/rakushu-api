using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public static class BankQuestionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"BANK_QUESTION.NOT_FOUND", "The bank question was not found.");

	public static readonly Error DuplicateQuestion = Error.Conflict(
		"BANK_QUESTION.DUPLICATE", "A question with the same text and type already exists for this vocabulary meaning.");

	public static readonly Error InvalidMeaning = Error.Validation(
		"BANK_QUESTION.INVALID_MEANING", "Knowledge meaning ID cannot be empty.");

	public static readonly Error InvalidType = Error.Validation(
		"BANK_QUESTION.INVALID_TYPE", "Question type ID cannot be empty.");

	public static readonly Error EmptyPrompt = Error.Validation(
		"BANK_QUESTION.EMPTY_PROMPT", "Question prompt cannot be empty.");

	public static readonly Error EmptyCorrectChoice = Error.Validation(
		"BANK_QUESTION.EMPTY_CORRECT_CHOICE", "Correct choice ID cannot be empty.");

	public static readonly Error ChoiceNotFound = Error.Validation(
		"BANK_QUESTION.CHOICE_NOT_FOUND", "The correct choice ID does not exist in the choices list.");

	public static readonly Error DuplicateChoiceId = Error.Validation(
		"BANK_QUESTION.DUPLICATE_CHOICE_ID", "Answer choice IDs must be unique.");

	public static readonly Error DuplicateChoiceText = Error.Validation(
		"BANK_QUESTION.DUPLICATE_CHOICE_TEXT", "Answer choices cannot have identical text.");

	public static readonly Error InvalidDifficulty = Error.Validation(
		"BANK_QUESTION.INVALID_DIFFICULTY", "Difficulty must be Easy, Medium, or Hard.");

	public static readonly Error MeaningNotFound = Error.NotFound(
		"BANK_QUESTION.MEANING_NOT_FOUND", "The specified knowledge meaning does not exist.");

	public static readonly Error QuestionTypeNotFound = Error.NotFound(
		"BANK_QUESTION.QUESTION_TYPE_NOT_FOUND", "The specified question type does not exist.");
}
