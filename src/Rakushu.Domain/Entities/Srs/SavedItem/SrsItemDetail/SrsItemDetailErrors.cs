using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;

public static class SrsItemDetailErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SRS_ITEM_DETAIL.NOT_FOUND", "The SRS item detail was not found.");

	public static readonly Error InvalidSrsScheduleId = Error.Validation(
		"SRS_ITEM_DETAIL.INVALID_SRS_SCHEDULE_ID", "SRS schedule ID cannot be empty.");

	public static readonly Error InvalidReviewNumber = Error.Validation(
		"SRS_ITEM_DETAIL.INVALID_REVIEW_NUMBER", "Review number must be greater than zero.");

	public static readonly Error InvalidQuality = Error.Validation(
		"SRS_ITEM_DETAIL.INVALID_QUALITY", "SM-2 quality rating must be between 0 and 5.");
}
