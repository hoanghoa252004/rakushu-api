using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the quiz-attempt aggregate and its items.
/// </summary>
public static class QuizAttemptErrors
{
    public static readonly Error InvalidUserId = Error.Validation("SRS.INVALID_USER_ID", "A valid user ID is required.");
    public static readonly Error InvalidCardId = Error.Validation("SRS.INVALID_CARD_ID", "A valid SRS card ID is required.");
    public static readonly Error InvalidQuizAnswer = Error.Validation("SRS.INVALID_QUIZ_ANSWER", "Quiz input is invalid or required answer data is missing.");
    public static readonly Error InvalidSelectedCards = Error.Validation("SRS.INVALID_SELECTED_CARDS", "Select between 2 and 50 unique, eligible SRS cards for this quiz.");
    public static readonly Error InvalidReviewTiming = Error.Validation("SRS.INVALID_REVIEW_TIMING", "Review time must not precede the item timestamp and elapsed time cannot be negative.");
    public static readonly Error RetryRequiresCompletedAttempt = Error.Validation("SRS.RETRY_REQUIRES_COMPLETED_ATTEMPT", "Only a completed quiz attempt can be retried.");
    public static readonly Error QuizNotInProgress = Error.Validation("SRS.QUIZ_NOT_IN_PROGRESS", "The quiz attempt is not in progress.");
    public static readonly Error DuplicateQuizCard = Error.Conflict("SRS.DUPLICATE_QUIZ_CARD", "The card or position is already present in this quiz.");
    public static readonly Error QuizAttemptNotFound = Error.NotFound("SRS.QUIZ_ATTEMPT_NOT_FOUND", "The quiz attempt was not found for the current user.");
    public static readonly Error QuizItemNotFound = Error.NotFound("SRS.QUIZ_ITEM_NOT_FOUND", "The quiz item was not found.");
    public static readonly Error QuizHasUnansweredItems = Error.Validation("SRS.QUIZ_HAS_UNANSWERED_ITEMS", "All quiz items must be answered before completion.");
    public static readonly Error QuizItemAlreadyAnswered = Error.Conflict("SRS.QUIZ_ITEM_ALREADY_ANSWERED", "The quiz item has already been answered.");
}
