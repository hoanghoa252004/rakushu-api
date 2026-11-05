using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
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

	private ShadowingMaterial() { }

	private ShadowingMaterial(
		ShadowingMaterialId id,
		MaterialType type,
		ShadowingMaterialRequestId? shadowingMaterialRequestId,
		VideoId videoId,
		int version,
		DateTimeOffset createdAt) : base(id)
	{
		Type = type;
		Status = ShadowingMaterialStatus.Processing;
		ShadowingMaterialRequestId = shadowingMaterialRequestId;
		VideoId = videoId;
		Version = version;
		CreatedAt = createdAt;
	}

	public static Result<ShadowingMaterial> Create(
		MaterialType type,
		ShadowingMaterialRequestId? shadowingMaterialRequestId,
		VideoId videoId,
		int version,
		DateTimeOffset createdAt)
	{
		if (!Enum.IsDefined(type))
			return Result.Failure<ShadowingMaterial>(ShadowingMaterialErrors.InvalidType);

		if (type == MaterialType.RequestedByLearner && shadowingMaterialRequestId is null)
			return Result.Failure<ShadowingMaterial>(ShadowingMaterialErrors.RequestRequired);

		if (type == MaterialType.Curated && shadowingMaterialRequestId is not null)
			return Result.Failure<ShadowingMaterial>(ShadowingMaterialErrors.CuratedCannotHaveRequest);

		return Result.Success(new ShadowingMaterial(
			ShadowingMaterialId.Create(),
			type,
			shadowingMaterialRequestId,
			videoId,
			version,
			createdAt));
	}

	public Result ChangeStatus(ShadowingMaterialStatus newStatus)
	{
		if (!ShadowingMaterialStatusTransition.IsAllowed(Status, newStatus))
			return Result.Failure(ShadowingMaterialErrors.InvalidStatusTransition);

		Status = newStatus;
		return Result.Success();
	}
}
