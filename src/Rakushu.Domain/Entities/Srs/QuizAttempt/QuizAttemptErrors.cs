using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the quiz-attempt aggregate and its items.
/// </summary>
public static class QuizAttemptErrors
{
	public static readonly Error InvalidUserId = Error.Validation("SRS.INVALID_USER_ID", "A valid user ID is required.");
	public static readonly Error InvalidItemId = Error.Validation("SRS.INVALID_ITEM_ID", "A valid SRS item ID is required.");
	public static readonly Error InvalidCardId = InvalidItemId;
	public static readonly Error InvalidQuizAnswer = Error.Validation("SRS.INVALID_QUIZ_ANSWER", "Quiz input is invalid or required answer data is missing.");
	public static readonly Error QuizGenerationFailed = Error.Failure("SRS.QUIZ_GENERATION_FAILED", "The quiz could not be generated safely. Please try again.");
	public static readonly Error UnsupportedQuestionFormat = Error.Validation("SRS.UNSUPPORTED_QUESTION_FORMAT", "Only MC quiz questions are currently supported.");
	public static readonly Error InvalidSelectedItems = Error.Validation("SRS.INVALID_SELECTED_ITEMS", "Select between 2 and 50 unique, eligible SRS items for this quiz.");
	public static readonly Error InvalidSelectedCards = InvalidSelectedItems;
	public static readonly Error InvalidReviewTiming = Error.Validation("SRS.INVALID_REVIEW_TIMING", "Review time must not precede the item timestamp and elapsed time cannot be negative.");
	public static readonly Error AttemptNotCompleted = Error.Validation("SRS.ATTEMPT_NOT_COMPLETED", "The quiz attempt has not been completed yet.");
	public static readonly Error QuizNotInProgress = Error.Validation("SRS.QUIZ_NOT_IN_PROGRESS", "The quiz attempt is not in progress.");
	public static readonly Error DuplicateQuizItem = Error.Conflict("SRS.DUPLICATE_QUIZ_ITEM", "The item or position is already present in this quiz.");
	public static readonly Error DuplicateQuizCard = DuplicateQuizItem;
	public static readonly Error QuizNotFound = Error.NotFound("SRS.QUIZ_NOT_FOUND", "The quiz was not found.");
	public static readonly Error QuizAttemptNotFound = Error.NotFound("SRS.QUIZ_ATTEMPT_NOT_FOUND", "The quiz attempt was not found for the current user.");
	public static readonly Error QuizItemNotFound = Error.NotFound("SRS.QUIZ_ITEM_NOT_FOUND", "The quiz item was not found.");
	public static readonly Error QuizHasUnansweredItems = Error.Validation("SRS.QUIZ_HAS_UNANSWERED_ITEMS", "All quiz items must be answered before completion.");
	public static readonly Error QuizItemAlreadyAnswered = Error.Conflict("SRS.QUIZ_ITEM_ALREADY_ANSWERED", "The quiz item has already been answered.");
	public static readonly Error InvalidQuestion = Error.Validation("SRS.INVALID_QUESTION", "A valid question is required.");
	public static readonly Error InvalidQuestionPosition = Error.Validation("SRS.INVALID_QUESTION_POSITION", "Position must be greater than 0.");
	public static readonly Error InvalidQuizPrompt = Error.Validation("SRS.INVALID_QUIZ_PROMPT", "A question prompt is required.");
	public static readonly Error InvalidCorrectOptionId = Error.Validation("SRS.INVALID_CORRECT_OPTION_ID", "A correct option ID is required.");
	public static readonly Error NoMistakesToRetry = Error.Validation("SRS.NO_MISTAKES_TO_RETRY", "There are no incorrect answers in this attempt to retry.");
	public static readonly Error InvalidRetryMode = Error.Validation("SRS.INVALID_RETRY_MODE", "Invalid retry mode. Supported modes are 'All' and 'MistakesOnly'.");
}
