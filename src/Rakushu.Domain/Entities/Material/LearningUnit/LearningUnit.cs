using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.LearningUnit;

public sealed class LearningUnit : AggregateRoot<LearningUnitId>
{
	public KnowledgeUnitId LinguisticKnowledgeId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public int StartTokenIndex { get; private set; }
	public int EndTokenIndex { get; private set; }
	public double Confidence { get; private set; }
	public MatchingMethod DetectionMethod { get; private set; }
	public DateTimeOffset DetectedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public Knowledge.KnowledgeUnit.KnowledgeUnit LinguisticKnowledge { get; private set; } = null!;

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private LearningUnit()
	{
	}
}
