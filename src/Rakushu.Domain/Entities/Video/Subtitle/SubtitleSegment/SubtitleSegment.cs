using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;

public sealed class SubtitleSegment : Entity<SubtitleSegmentId>
{
	
	public SubtitleId SubtitleId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public string OriginalText { get; private set; } = null!;
	public string TranslatedText { get; private set; } = null!;
	public int Sequence { get; private set; }
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Subtitle
	public Subtitle Subtitle { get; private set; } = null!;

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS

	private SubtitleSegment()
	{
	}

	private SubtitleSegment(
		SubtitleSegmentId id,
		SubtitleId subtitleId,
		TranscriptSegmentId transcriptSegmentId,
		string originalText,
		string translatedText,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		SubtitleId = subtitleId;
		TranscriptSegmentId = transcriptSegmentId;
		OriginalText = originalText;
		TranslatedText = translatedText;
		StartTime = startTime;
		EndTime = endTime;
		Sequence = sequence;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}