using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticKnowledge;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit;

public sealed class LearningUnit : AggregateRoot<LearningUnitId>
{
	public LinguisticKnowledgeId LinguisticKnowledgeId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public int StartTokenIndex { get; private set; }
	public int EndTokenIndex { get; private set; }
	public double Confidence { get; private set; }
	public KnowledgeDetectionMethod DetectionMethod { get; private set; }
	public DateTimeOffset DetectedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public LinguisticKnowledge.LinguisticKnowledge LinguisticKnowledge { get; private set; } = null!;

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private LearningUnit()
	{
	}

	private LearningUnit(
		LearningUnitId id,
		LinguisticKnowledgeId linguisticKnowledgeId,
		TranscriptSegmentId transcriptSegmentId,
		int startTokenIndex,
		int endTokenIndex,
		double confidence,
		KnowledgeDetectionMethod detectionMethod,
		DateTimeOffset detectedAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		LinguisticKnowledgeId = linguisticKnowledgeId;
		TranscriptSegmentId = transcriptSegmentId;
		StartTokenIndex = startTokenIndex;
		EndTokenIndex = endTokenIndex;
		Confidence = confidence;
		DetectionMethod = detectionMethod;
		DetectedAt = detectedAt;
		UpdatedAt = updatedAt;
	}
}
