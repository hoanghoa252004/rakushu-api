using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public sealed class ShadowingMaterialRequest : Entity<ShadowingMaterialRequestId>
{
	public string Code { get; private set; } = null!; // PATTERN: SMR-0001
	public VideoId VideoId { get; private set; } = null!;
	public UserId LearnerId { get; private set; } = null!;
	public ShadowingMaterialRequestStatus Status { get; private set; }
	public string? RejectReason { get; private set; }
	public DateTimeOffset RequestedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// Learner
	public User Learner { get; private set; } = null!;

	// ShadowingMaterials
	private readonly List<ShadowingMaterial.ShadowingMaterial> _shadowingMaterials = new();
	public IReadOnlyCollection<ShadowingMaterial.ShadowingMaterial> ShadowingMaterials => _shadowingMaterials.AsReadOnly();

	private ShadowingMaterialRequest() { }
}