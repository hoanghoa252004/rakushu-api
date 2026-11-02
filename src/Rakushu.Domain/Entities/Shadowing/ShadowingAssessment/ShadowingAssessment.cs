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
	public UserId? EvaluatorId { get; private set; }
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
}
