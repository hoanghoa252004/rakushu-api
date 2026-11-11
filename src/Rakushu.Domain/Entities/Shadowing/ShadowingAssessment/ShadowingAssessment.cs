using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;

public sealed class ShadowingAssessment : Entity<ShadowingAssessmentId>
{
	public ShadowingAttemptId ShadowingAttemptId { get; private set; } = null!;
	public ShadowingAssessmentType Type { get; private set; }

	// Null for automated assessment.
	public decimal? OverallScore { get; private set; }
	public string? OveralFeedback { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingAttempt
	public ShadowingAttempt.ShadowingAttempt ShadowingAttempt { get; private set; } = null!;

	// ShadowingAssessmentDetails
	private readonly List<ShadowingAssessmentDetail.ShadowingAssessmentDetail> _details = new();
	public IReadOnlyCollection<ShadowingAssessmentDetail.ShadowingAssessmentDetail> Details => _details.AsReadOnly();

	private ShadowingAssessment() { }

	private ShadowingAssessment(ShadowingAttemptId attemptId, DateTimeOffset now)
		: base(ShadowingAssessmentId.Create())
	{
		ShadowingAttemptId = attemptId;
		Type = ShadowingAssessmentType.Automated;
		CreatedAt = now;
	}

	public static ShadowingAssessment CreateAutomated(ShadowingAttemptId attemptId, DateTimeOffset now)
		=> new(attemptId, now);

	public void AddDetail(
		ShadowingAttempt.ShadowingAttemptDetail.ShadowingAttemptDetailId attemptDetailId,
		decimal accuracyScore,
		string? accuracyResponse,
		decimal pitchScore,
		string? pitchResponse)
	{
		_details.Add(new ShadowingAssessmentDetail.ShadowingAssessmentDetail(
			Id, attemptDetailId, accuracyScore, accuracyResponse, pitchScore, pitchResponse));
	}

	// Overall score = average of the (accuracy + pitch) / 2 of every detail.
	public void CalculateOverallScore()
	{
		var scores = _details
			.Select(d => ((d.AccuracyScore ?? 0m) + (d.PitchSimilarityScore ?? 0m)) / 2m)
			.ToList();
		OverallScore = scores.Count == 0 ? null : Math.Round(scores.Average(), 2);
	}
}
