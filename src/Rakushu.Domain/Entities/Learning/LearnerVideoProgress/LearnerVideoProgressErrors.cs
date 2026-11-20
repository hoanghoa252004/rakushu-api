using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Learning.LearnerVideoProgress;

public static class LearnerVideoProgressErrors
{
	public static readonly Error InvalidUserId = Error.Validation(
		"VIDEO_PROGRESS.INVALID_USER_ID", "The user ID cannot be empty.");

	public static readonly Error InvalidVideoId = Error.Validation(
		"VIDEO_PROGRESS.INVALID_VIDEO_ID", "The video ID cannot be empty.");

	public static readonly Error InvalidWatchedRange = Error.Validation(
		"VIDEO_PROGRESS.INVALID_WATCHED_RANGE", "The watched range start time must be non-negative and not exceed end time.");

	public static readonly Error NotFound = Error.NotFound(
		"VIDEO_PROGRESS.NOT_FOUND", "The video watch progress record was not found.");
}
