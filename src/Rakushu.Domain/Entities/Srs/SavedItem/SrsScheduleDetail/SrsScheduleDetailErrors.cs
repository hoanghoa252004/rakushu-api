using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

public static class SrsScheduleDetailErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SrsScheduleDetail.NotFound", "Schedule review detail was not found.");

	public static readonly Error InvalidSrsScheduleId = Error.Validation(
		"SrsScheduleDetail.InvalidSrsScheduleId", "Schedule ID must be a valid non-empty GUID.");

	public static readonly Error InvalidReviewNumber = Error.Validation(
		"SrsScheduleDetail.InvalidReviewNumber", "Review number must be at least 1.");

	public static readonly Error InvalidQuality = Error.Validation(
		"SrsScheduleDetail.InvalidQuality", "Quality must be between 0 and 5.");
}
