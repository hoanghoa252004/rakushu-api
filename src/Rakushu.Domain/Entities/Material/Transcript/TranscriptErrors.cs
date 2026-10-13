using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Material.Transcript;

public static class TranscriptErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"TRANSCRIPT.NOT_FOUND", "The transcript was not found.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"TRANSCRIPT.ALREADY_EXISTS",
		"This video already has a transcript. Only one transcript per video is allowed.");

	public static readonly Error CannotDeleteReadyVideo = Error.Conflict(
		"TRANSCRIPT.CANNOT_DELETE_READY_VIDEO",
		"Cannot delete transcript from a video in Ready status. Only update is allowed for Ready videos.");
}
