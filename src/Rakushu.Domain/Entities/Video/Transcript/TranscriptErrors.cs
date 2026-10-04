using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Video.Transcript;

public static class TranscriptErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"TRANSCRIPT.NOT_FOUND", "The transcript was not found.");
}
