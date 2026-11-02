using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

public sealed class ShadowingAttemptDetail : Entity<ShadowingAttemptDetailId>
{
	public ShadowingAttemptId ShadowingAttemptId { get; private set; } = null!;
	public ShadowingRoleLineId ShadowingRoleLineId { get; private set; } = null!;
	public string? AudioKey { get; private set; }
	public long? DurationMs { get; private set; }
	public DateTimeOffset? SubmittedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public ShadowingAttempt ShadowingAttempt { get; private set; } = null!;

	public ShadowingRoleLine.ShadowingRoleLine ShadowingRoleLine { get; private set; } = null!;

	private readonly List<ShadowingAssessment.ShadowingAssessmentDetail.ShadowingAssessmentDetail> _assessmentDetails
		= new();

	public IReadOnlyCollection<ShadowingAssessment.ShadowingAssessmentDetail.ShadowingAssessmentDetail>
		AssessmentDetails => _assessmentDetails.AsReadOnly();

	private ShadowingAttemptDetail() { }
}
