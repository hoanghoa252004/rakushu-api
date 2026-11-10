namespace Rakushu.Application.Abstractions.Infrastructure.Realtime;

public sealed record RoleLineCompletedMessage(
	Guid SessionId,
	Guid AttemptId,
	Guid RoleLineId,
	Guid MemberId,
	bool IsSkipped,
	DateTimeOffset CompletedAt);

public interface IShadowingRealtimeNotifier
{
	Task NotifyRoleLineCompletedAsync(RoleLineCompletedMessage message, CancellationToken cancellationToken);
}
