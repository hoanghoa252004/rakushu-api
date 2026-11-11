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
	// Raw response from the accuracy assessment service.
	public string? AccuracyAssessmentResponse { get; private set; }
	// 0 - 100
	public decimal? PitchSimilarityScore { get; private set; }
	// Raw response from the pitch assessment service.
	public string? PitchSimilarityAssessmentResponse { get; private set; }
	public string? Feedback { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingAssessment
	public ShadowingAssessment ShadowingAssessment { get; private set; } = null!;

	public ShadowingAttemptDetail ShadowingAttemptDetail { get; private set; } = null!;

	// CONSTRUCTORS
	private ShadowingAssessmentDetail() { }

	internal ShadowingAssessmentDetail(
		ShadowingAssessmentId assessmentId,
		ShadowingAttemptDetailId attemptDetailId,
		decimal? accuracyScore,
		string? accuracyResponse,
		decimal? pitchScore,
		string? pitchResponse) : base(ShadowingAssessmentDetailId.Create())
	{
		ShadowingAssessmentId = assessmentId;
		ShadowingAttemptDetailId = attemptDetailId;
		AccuracyScore = accuracyScore;
		AccuracyAssessmentResponse = accuracyResponse;
		PitchSimilarityScore = pitchScore;
		PitchSimilarityAssessmentResponse = pitchResponse;
	}
}
