using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Video;

public static class VideoErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"VIDEO.NOT_FOUND", "The video was not found.");

	public static readonly Error InvalidTitle = Error.Validation(
		"VIDEO.INVALID_TITLE", "Video title is required and cannot exceed 200 characters.");

	public static readonly Error InvalidSlug = Error.Validation(
		"VIDEO.INVALID_SLUG", "Video slug is required and cannot exceed 200 characters.");
}
