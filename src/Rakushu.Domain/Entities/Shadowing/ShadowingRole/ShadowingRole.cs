using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRole;

public sealed class ShadowingRole : Entity<ShadowingRoleId>
{
	public ShadowingMaterialId ShadowingMaterialId { get; private set; } = null!;
	public SpeakerId SpeakerId { get; private set; } = null!;
	public int SortOrder { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingMaterial
	public ShadowingMaterial.ShadowingMaterial ShadowingMaterial { get; private set; } = null!;

	// Speaker
	public Speaker.Speaker Speaker { get; private set; } = null!;

	// ShadowingMembers
	private readonly List<ShadowingSessionMember.ShadowingSessionMember> _members = new();
	public IReadOnlyCollection<ShadowingSessionMember.ShadowingSessionMember> Members => _members.AsReadOnly();

	// ShadowingRoleLines
	private readonly List<ShadowingRoleLine.ShadowingRoleLine> _roleLines = new();
	public IReadOnlyCollection<ShadowingRoleLine.ShadowingRoleLine> RoleLines => _roleLines.AsReadOnly();
}
