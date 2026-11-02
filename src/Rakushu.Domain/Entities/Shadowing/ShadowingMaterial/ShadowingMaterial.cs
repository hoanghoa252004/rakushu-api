using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public sealed class ShadowingMaterial : Entity<ShadowingMaterialId>
{
	public string Code { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;
	public int Version { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ShadowingRoles
	private readonly List<ShadowingRole.ShadowingRole> _roles = new();
	public IReadOnlyCollection<ShadowingRole.ShadowingRole> Roles => _roles.AsReadOnly();

	// ShadowingRoleLines
	private readonly List<ShadowingRoleLine.ShadowingRoleLine> _roleLines = new();

	public IReadOnlyCollection<ShadowingRoleLine.ShadowingRoleLine> RoleLines => _roleLines.AsReadOnly();

	// ShadowingSessions
	private readonly List<ShadowingSession.ShadowingSession> _sessions = new();
	public IReadOnlyCollection<ShadowingSession.ShadowingSession> Sessions => _sessions.AsReadOnly();

}
