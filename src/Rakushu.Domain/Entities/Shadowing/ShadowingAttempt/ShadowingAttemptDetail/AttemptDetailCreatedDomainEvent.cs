using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

public sealed record AttemptDetailCreatedDomainEvent(
	Guid SessionId,
	Guid AttemptId,
	Guid RoleLineId,
	Guid MemberId,
	bool IsSkipped,
	DateTimeOffset CompletedAt) : DomainEvent;
