using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;

public sealed class TranscriptItem : Entity<TranscriptItemId>
{
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public TranscriptItemType Type { get; private set; }
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public decimal Confidence { get; private set; }

	// NAVIGATION PROPERTIES
	// TranscriptSegment
	public TranscriptSegment.TranscriptSegment TranscriptSegment { get; private set; } = null!;

	private TranscriptItem() { }
}