using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the SRS review-session aggregate and its items.
/// </summary>
public static class SrsSessionErrors
{
    public static readonly Error InvalidUserId = Error.Validation("SRS.INVALID_USER_ID", "A valid user ID is required.");
    public static readonly Error InvalidCardId = Error.Validation("SRS.INVALID_CARD_ID", "A valid SRS card ID is required.");
    public static readonly Error InvalidReviewRating = Error.Validation("SRS.INVALID_REVIEW_RATING", "Rating must be Again, Hard, Good, or Easy.");
    public static readonly Error InvalidReviewTiming = Error.Validation("SRS.INVALID_REVIEW_TIMING", "Review time must not precede the item timestamp and elapsed time cannot be negative.");
    public static readonly Error SessionNotInProgress = Error.Validation("SRS.SESSION_NOT_IN_PROGRESS", "The SRS session is not in progress.");
    public static readonly Error DuplicateSessionCard = Error.Conflict("SRS.DUPLICATE_SESSION_CARD", "The card is already in this session.");
    public static readonly Error SessionNotFound = Error.NotFound("SRS.SESSION_NOT_FOUND", "The SRS session was not found.");
    public static readonly Error SessionItemNotFound = Error.NotFound("SRS.SESSION_ITEM_NOT_FOUND", "The session item was not found.");
    public static readonly Error SessionHasUnreviewedItems = Error.Validation("SRS.SESSION_HAS_UNREVIEWED_ITEMS", "All session items must be reviewed before completion.");
    public static readonly Error SessionItemAlreadyReviewed = Error.Conflict("SRS.SESSION_ITEM_ALREADY_REVIEWED", "The session item has already been reviewed.");
}
