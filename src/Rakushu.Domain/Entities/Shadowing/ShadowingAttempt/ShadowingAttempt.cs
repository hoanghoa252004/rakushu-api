using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

public sealed class ShadowingAttempt : Entity<ShadowingAttemptId>
{
	public ShadowingSessionId ShadowingSessionId { get; private set; } = null!;
	public ShadowingSessionMemberId ShadowingSessionMemberId { get; private set; } = null!;
	public ShadowingAttemptStatus Status { get; private set; }
	public DateTimeOffset StartedAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingSession
	public ShadowingSession.ShadowingSession ShadowingSession { get; private set; } = null!;

	// ShadowingSessionMember
	public ShadowingSessionMember.ShadowingSessionMember ShadowingSessionMember { get; private set; } = null!;

	private readonly List<ShadowingAttemptDetail.ShadowingAttemptDetail> _details = new();

	public IReadOnlyCollection<ShadowingAttemptDetail.ShadowingAttemptDetail> Details => _details.AsReadOnly();

	private readonly List<ShadowingAssessment.ShadowingAssessment> _assessments = new();

	public IReadOnlyCollection<ShadowingAssessment.ShadowingAssessment> Assessments => _assessments.AsReadOnly();

	private ShadowingAttempt() { }
}
