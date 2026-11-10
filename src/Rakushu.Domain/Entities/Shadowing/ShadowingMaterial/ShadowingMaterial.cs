using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public sealed class ShadowingMaterial : Entity<ShadowingMaterialId>
{
	public string Name { get; private set; } = null!;
	public TranscriptSegmentId? StartTranscriptSegmentId { get; private set; }
	public TranscriptSegmentId? EndTranscriptSegmentId { get; private set; }
	public ShadowingMaterialStatus Status { get; private set; }
	public VideoId VideoId { get; private set; } = null!;
	public int Version { get; private set; }
	public UserId CreatedById { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// CreatedBy
	public User CreatedBy { get; private set; } = null!;

	// Video
	public Video Video { get; private set; } = null!;

	// StartTranscriptSegment
	public TranscriptSegment StartTranscriptSegment { get; private set; } = null!;

	// EndTranscriptSegment
	public TranscriptSegment EndTranscriptSegment { get; private set; } = null!;

	// ShadowingRoles
	private readonly List<ShadowingRole.ShadowingRole> _roles = new();
	public IReadOnlyCollection<ShadowingRole.ShadowingRole> Roles => _roles.AsReadOnly();

	// ShadowingSessions
	private readonly List<ShadowingSession.ShadowingSession> _sessions = new();
	public IReadOnlyCollection<ShadowingSession.ShadowingSession> Sessions => _sessions.AsReadOnly();

	private ShadowingMaterial() { }

	private ShadowingMaterial(
		ShadowingMaterialId id,
		string name,
		TranscriptSegmentId startSegmentId,
		TranscriptSegmentId endSegmentId,
		VideoId videoId,
		int version,
		UserId createdById,
		DateTimeOffset createdAt) : base(id)
	{
		Name = name;
		StartTranscriptSegmentId = startSegmentId;
		EndTranscriptSegmentId = endSegmentId;
		Status = ShadowingMaterialStatus.Processing;
		VideoId = videoId;
		Version = version;
		CreatedById = createdById;
		CreatedAt = createdAt;
	}

	public static Result<ShadowingMaterial> Create(
		string name,
		TranscriptSegmentId startSegmentId,
		TranscriptSegmentId endSegmentId,
		VideoId videoId,
		int version,
		UserId createdById,
		DateTimeOffset createdAt)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
			return Result.Failure<ShadowingMaterial>(ShadowingMaterialErrors.InvalidName);

		return Result.Success(new ShadowingMaterial(
			ShadowingMaterialId.Create(),
			name.Trim(),
			startSegmentId,
			endSegmentId,
			videoId,
			version,
			createdById,
			createdAt));
	}

	public Result Rename(string name)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
			return Result.Failure(ShadowingMaterialErrors.InvalidName);

		Name = name.Trim();
		return Result.Success();
	}

	public void SetRange(TranscriptSegmentId startSegmentId, TranscriptSegmentId endSegmentId)
	{
		StartTranscriptSegmentId = startSegmentId;
		EndTranscriptSegmentId = endSegmentId;
	}

	public Result ChangeStatus(ShadowingMaterialStatus newStatus)
	{
		if (!ShadowingMaterialStatusTransition.IsAllowed(Status, newStatus))
			return Result.Failure(ShadowingMaterialErrors.InvalidStatusTransition);

		Status = newStatus;
		return Result.Success();
	}
}
