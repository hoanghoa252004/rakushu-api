using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

public static class TranscriptSegmentErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"TRANSCRIPT_SEGMENT.NOT_FOUND", "The transcript segment was not found.");
}
