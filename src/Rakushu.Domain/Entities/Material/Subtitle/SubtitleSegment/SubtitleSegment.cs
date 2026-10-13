using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Subtitle.SubtitleSegment;

public sealed class SubtitleSegment : Entity<SubtitleSegmentId>
{
	public int Sequence { get; private set; }
	public SubtitleId SubtitleId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public string OriginalText { get; private set; } = null!;
	public string TranslatedText { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }

	// NAVIGATION PROPERTIES
	// Subtitle
	public Subtitle Subtitle { get; private set; } = null!;

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS

	private SubtitleSegment()
	{
	}
}