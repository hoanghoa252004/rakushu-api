using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;

using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.TranscriptSegment;

public sealed record TranscriptSegmentDto(
	Guid TranscriptId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int Sequence)
{
	public static TranscriptSegmentDto FromEntity(Entity e) =>
		new(
			e.TranscriptId.Value,
			e.Text,
			e.StartTime,
			e.EndTime,
			e.Sequence);
}
