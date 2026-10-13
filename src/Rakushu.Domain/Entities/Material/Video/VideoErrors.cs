using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Material.Video;

public static class VideoErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"VIDEO.NOT_FOUND", "The video was not found.");

	public static readonly Error InvalidTitle = Error.Validation(
		"VIDEO.INVALID_TITLE", "Video title is required and cannot exceed 200 characters.");

	public static readonly Error InvalidSlug = Error.Validation(
		"VIDEO.INVALID_SLUG", "Video slug is required and cannot exceed 200 characters.");

	public static readonly Error InvalidVideoSource = Error.Validation(
		"VIDEO.INVALID_VIDEO_SOURCE", "Invalid video source.");

	public static readonly Error InvalidVideoStatus = Error.Validation(
		"VIDEO.INVALID_VIDEO_STATUS", "Invalid video status.");

	public static readonly Error InvalidStatusTransition = Error.Validation(
		"VIDEO.INVALID_STATUS_TRANSITION", "Invalid status transition for video.");

	public static readonly Error UnauthorizedVideoSourceForCurator = Error.Validation(
		"VIDEO.UNAUTHORIZED_VIDEO_SOURCE", "Only curators can upload videos with Curated source.");

	public static readonly Error CannotDeleteVideo = Error.Validation(
		"VIDEO.CANNOT_DELETE_VIDEO", "Video cannot be deleted in current status.");

	public static readonly Error CannotUpdateVideoStatus = Error.Validation(
		"VIDEO.CANNOT_UPDATE_VIDEO_STATUS", "User is not authorized to update video status.");
}
