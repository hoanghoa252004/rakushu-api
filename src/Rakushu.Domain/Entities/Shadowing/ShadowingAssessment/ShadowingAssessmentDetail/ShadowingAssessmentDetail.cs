using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAssessment.ShadowingAssessmentDetail;

public sealed class ShadowingAssessmentDetail : Entity<ShadowingAssessmentDetailId>
{
	public ShadowingAssessmentId ShadowingAssessmentId { get; private set; } = null!;
	public ShadowingAttemptDetailId ShadowingAttemptDetailId { get; private set; } = null!;
	// 0 - 100
	public decimal? AccuracyScore { get; private set; }
	// 0 - 100
	public decimal? TimingScore { get; private set; }
	// 0 - 100
	public decimal? PitchSimilarityScore { get; private set; }
	// 0 - 100
	public decimal? IntensitySimilarityScore { get; private set; }
	public string? Feedback { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingAssessment
	public ShadowingAssessment ShadowingAssessment { get; private set; } = null!;

	public ShadowingAttemptDetail ShadowingAttemptDetail { get; private set; } = null!;

	// CONSTRUCTORS
	private ShadowingAssessmentDetail() { }
}
