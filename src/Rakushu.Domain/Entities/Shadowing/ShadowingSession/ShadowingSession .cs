using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public sealed class ShadowingSession : Entity<ShadowingSessionId>
{
	public ShadowingMaterialId ShadowingMaterialId { get; private set; } = null!;
	public UserId HostId { get; private set; } = null!;
	public string Title { get; private set; } = null!;
	public ShadowingSessionVisibility Visibility { get; private set; }
	public ShadowingSessionStatus Status { get; private set; }
	public DateTimeOffset ScheduledStartAt { get; private set; }
	public DateTimeOffset? StartedAt { get; private set; }
	public DateTimeOffset? EndedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingMaterial
	public ShadowingMaterial.ShadowingMaterial ShadowingMaterial { get; private set; } = null!;
	
	// ShadowingSessionMembers
	private readonly List<ShadowingSessionMember.ShadowingSessionMember> _members = new();
	public IReadOnlyCollection<ShadowingSessionMember.ShadowingSessionMember> Members => _members.AsReadOnly();

	// ShadowingAttempts
	private readonly List<ShadowingAttempt.ShadowingAttempt> _attempts = new();
	public IReadOnlyCollection<ShadowingAttempt.ShadowingAttempt> Attempts => _attempts.AsReadOnly();

	private ShadowingSession() { }
}
