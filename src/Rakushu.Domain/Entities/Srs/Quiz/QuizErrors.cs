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

	public static readonly Error NoDueItemsToReview = Error.NotFound(
		"QUIZ.NO_DUE_ITEMS", "There are no due SRS items to review at this moment.");

	public static readonly Error EmptySelectedItems = Error.Validation(
		"QUIZ.EMPTY_SELECTED_ITEMS", "At least one item must be selected to create a quiz.");

	public static readonly Error NoQuestionsAvailable = Error.NotFound(
		"QUIZ.NO_QUESTIONS_AVAILABLE", "No bank questions found for the selected items.");
}
