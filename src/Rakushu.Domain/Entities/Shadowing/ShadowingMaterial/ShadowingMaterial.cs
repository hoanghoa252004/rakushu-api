using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public sealed class ShadowingMaterial : Entity<ShadowingMaterialId>
{
	public MaterialType Type { get; private set; }
	public ShadowingMaterialStatus Status { get; private set; }
	public ShadowingMaterialRequestId? ShadowingMaterialRequestId { get; private set; }
	public VideoId VideoId { get; private set; } = null!;
	public int Version { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// ShadowingMaterialRequest
	public ShadowingMaterialRequest.ShadowingMaterialRequest? ShadowingMaterialRequest { get; private set; }

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
