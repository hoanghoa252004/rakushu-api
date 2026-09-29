using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

public sealed class TranscriptSegment : Entity<TranscriptSegmentId>
{
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public TranscriptId TranscriptId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Transcript
	public Transcript Transcript { get; private set; } = null!;

	// Bunsetsu
	private readonly List<Bunsetsu> _bunsetsu = new();
	public IReadOnlyCollection<Bunsetsu> Bunsetsu => _bunsetsu.AsReadOnly();

	// LearningUnits
	private readonly List<LearningUnit.LearningUnit> _learningUnits = new();
	public IReadOnlyCollection<LearningUnit.LearningUnit> LearningUnits => _learningUnits.AsReadOnly();

	// SubtitleSegment
	public SubtitleSegment SubtitleSegment { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private TranscriptSegment()
	{
	}

	private TranscriptSegment(
		TranscriptSegmentId id,
		TranscriptId transcriptId,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		TranscriptId = transcriptId;
		Text = text;
		StartTime = startTime;
		EndTime = endTime;
		Sequence = sequence;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
