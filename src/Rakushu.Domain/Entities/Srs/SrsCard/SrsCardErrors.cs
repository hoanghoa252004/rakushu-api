using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the SRS card aggregate.
/// </summary>
public static class SrsCardErrors
{
    public static readonly Error InvalidNoteId = Error.Validation("SRS.INVALID_NOTE_ID", "A valid SRS note ID is required.");
    public static readonly Error InvalidCardId = Error.Validation("SRS.INVALID_CARD_ID", "A valid SRS card ID is required.");
    public static readonly Error InvalidReviewRating = Error.Validation("SRS.INVALID_REVIEW_RATING", "Rating must be Again, Hard, Good, or Easy.");
    public static readonly Error InvalidCardStatusTransition = Error.Validation("SRS.INVALID_CARD_STATUS_TRANSITION", "The SRS card cannot transition from its current status.");
    public static readonly Error CardAlreadyArchived = Error.Conflict("SRS.CARD_ALREADY_ARCHIVED", "The SRS card is already archived.");
    public static readonly Error CardStateAlreadyExists = Error.Conflict("SRS.CARD_STATE_ALREADY_EXISTS", "The SRS card already has scheduling state.");
    public static readonly Error CardStateOwnerMismatch = Error.Validation("SRS.CARD_STATE_OWNER_MISMATCH", "The scheduling state belongs to a different card.");
    public static readonly Error CardNotReviewable = Error.Validation("SRS.CARD_NOT_REVIEWABLE", "The SRS card cannot be reviewed in its current status.");
    public static readonly Error CardStateMissing = Error.Validation("SRS.CARD_STATE_MISSING", "The SRS card has no scheduling state.");
    public static readonly Error NoDueCards = Error.NotFound("SRS.NO_DUE_CARDS", "There are no SRS cards due for review.");
}
