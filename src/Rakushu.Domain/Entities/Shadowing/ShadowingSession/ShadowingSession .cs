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

	private ShadowingSession(
		ShadowingSessionId id,
		ShadowingMaterialId materialId,
		UserId hostId,
		string title,
		ShadowingSessionVisibility visibility,
		DateTimeOffset scheduledStartAt) : base(id)
	{
		ShadowingMaterialId = materialId;
		HostId = hostId;
		Title = title;
		Visibility = visibility;
		Status = ShadowingSessionStatus.Scheduled;
		ScheduledStartAt = scheduledStartAt;
	}

	public static Domain.Common.Results.Result<ShadowingSession> Create(
		ShadowingMaterialId materialId,
		UserId hostId,
		string title,
		ShadowingSessionVisibility visibility,
		DateTimeOffset scheduledStartAt,
		DateTimeOffset now)
	{
		var validation = Validate(title, visibility, scheduledStartAt, now);
		if (validation.IsFailure)
			return Domain.Common.Results.Result.Failure<ShadowingSession>(validation.Error);

		var session = new ShadowingSession(
			ShadowingSessionId.Create(), materialId, hostId, title.Trim(), visibility, scheduledStartAt);
		session._members.Add(ShadowingSessionMember.ShadowingSessionMember.CreateHost(session.Id, hostId, now));
		return Domain.Common.Results.Result.Success(session);
	}

	public Domain.Common.Results.Result Update(
		string title,
		ShadowingSessionVisibility visibility,
		DateTimeOffset scheduledStartAt,
		DateTimeOffset now)
	{
		if (Status != ShadowingSessionStatus.Scheduled)
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.NotScheduled);

		var validation = Validate(title, visibility, scheduledStartAt, now);
		if (validation.IsFailure)
			return validation;

		Title = title.Trim();
		Visibility = visibility;
		ScheduledStartAt = scheduledStartAt;
		return Domain.Common.Results.Result.Success();
	}

	public Domain.Common.Results.Result Cancel()
	{
		if (Status != ShadowingSessionStatus.Scheduled)
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.NotScheduled);

		Status = ShadowingSessionStatus.Cancelled;
		return Domain.Common.Results.Result.Success();
	}

	public Domain.Common.Results.Result Start(DateTimeOffset now)
	{
		if (Status != ShadowingSessionStatus.Scheduled)
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.NotScheduled);

		Status = ShadowingSessionStatus.InProgress;
		StartedAt = now;
		return Domain.Common.Results.Result.Success();
	}

	public Domain.Common.Results.Result Finish(DateTimeOffset now, bool completed)
	{
		if (Status != ShadowingSessionStatus.InProgress)
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.NotInProgress);

		Status = completed ? ShadowingSessionStatus.Completed : ShadowingSessionStatus.Cancelled;
		EndedAt = now;
		return Domain.Common.Results.Result.Success();
	}

	private static Domain.Common.Results.Result Validate(
		string title,
		ShadowingSessionVisibility visibility,
		DateTimeOffset scheduledStartAt,
		DateTimeOffset now)
	{
		if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 200)
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.InvalidTitle);
		if (!Enum.IsDefined(visibility))
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.InvalidVisibility);
		if (scheduledStartAt < now.AddMinutes(10))
			return Domain.Common.Results.Result.Failure(ShadowingSessionErrors.StartTimeTooSoon);
		return Domain.Common.Results.Result.Success();
	}
}
