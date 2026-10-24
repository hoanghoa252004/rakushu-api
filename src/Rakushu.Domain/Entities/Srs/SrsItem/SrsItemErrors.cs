using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the SRS item aggregate.
/// </summary>
public static class SrsItemErrors
{
	public static readonly Error InvalidUserId = Error.Validation("SRS.INVALID_USER_ID", "A valid user ID is required.");
	public static readonly Error InvalidKnowledgeMeaningId = Error.Validation("SRS.INVALID_KNOWLEDGE_MEANING_ID", "A valid knowledge meaning ID is required.");
	public static readonly Error InvalidExpression = Error.Validation("SRS.INVALID_EXPRESSION", "A valid expression is required.");
	public static readonly Error InvalidMeaning = Error.Validation("SRS.INVALID_MEANING", "A valid meaning is required.");
	public static readonly Error InvalidItemId = Error.Validation("SRS.INVALID_ITEM_ID", "A valid SRS item ID is required.");
	public static readonly Error InvalidReviewRating = Error.Validation("SRS.INVALID_REVIEW_RATING", "Rating must be between 0 and 5.");
	public static readonly Error InvalidItemStatusTransition = Error.Validation("SRS.INVALID_ITEM_STATUS_TRANSITION", "The SRS item cannot transition from its current status.");
	public static readonly Error ItemAlreadyIgnored = Error.Conflict("SRS.ITEM_ALREADY_IGNORED", "The SRS item is already ignored.");
	public static readonly Error ItemNotReviewable = Error.Validation("SRS.ITEM_NOT_REVIEWABLE", "The SRS item cannot be reviewed in its current status.");
	public static readonly Error NoDueItems = Error.NotFound("SRS.NO_DUE_ITEMS", "There are no SRS items due for review.");
}
