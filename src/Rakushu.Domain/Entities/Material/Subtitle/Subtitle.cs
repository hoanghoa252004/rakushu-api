using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Subtitle;

public sealed class Subtitle : Entity<SubtitleId>
{
	public int Sequence { get; private set; }
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public LanguageCode LanguageCode { get; private set; }
	public string Text { get; private set; } = null!;

	// NAVIGATION PROPERTIES

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS

	private Subtitle()
	{
	}
}