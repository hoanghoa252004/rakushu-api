using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.Speaker;

public static class SpeakerErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SPEAKER.NOT_FOUND",
		"The speaker was not found.");

	public static readonly Error InvalidName = Error.Validation(
		"SPEAKER.INVALID_NAME",
		"Speaker name is required and cannot exceed 200 characters.");

	public static readonly Error InvalidDescription = Error.Validation(
		"SPEAKER.INVALID_DESCRIPTION",
		"Speaker description cannot exceed 1000 characters.");

	public static readonly Error InvalidThemeColor = Error.Validation(
		"SPEAKER.INVALID_THEME_COLOR",
		"Speaker theme color must be a valid hexadecimal color code (e.g., #3B82F6).");

	public static readonly Error CodeAlreadyExists = Error.Conflict(
		"SPEAKER.CODE_ALREADY_EXISTS",
		"A speaker with this code already exists.");

	public static readonly Error InUse = Error.Conflict(
		"SPEAKER.IN_USE",
		"The speaker is used by transcripts and cannot be deleted.");
}