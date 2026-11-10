using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public interface IShadowingSessionRepository : IBaseRepository<ShadowingSession, ShadowingSessionId>
{
	Task<bool> HasActiveHostedSessionAsync(
		UserId hostId,
		CancellationToken cancellationToken = default);

	Task<bool> HasScheduleConflictAsync(
		UserId userId,
		DateTimeOffset start,
		TimeSpan duration,
		ShadowingSessionId? excludeSessionId,
		CancellationToken cancellationToken = default);

	Task<TimeSpan?> GetDurationAsync(
		ShadowingSessionId sessionId,
		CancellationToken cancellationToken = default);

	// Tracked session with Members and Attempts loaded.
	Task<ShadowingSession?> GetWithAttemptsAsync(
		ShadowingSessionId id,
		CancellationToken cancellationToken = default);

	// Tracked InProgress sessions whose attempts are all Submitted/Cancelled with at least one Submitted... see implementation.
	Task<IReadOnlyList<ShadowingSession>> GetExpiredScheduledAsync(
		DateTimeOffset now, int batchSize, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ShadowingSession>> GetInProgressWithAttemptsAsync(
		int batchSize,
		CancellationToken cancellationToken = default);

	Task<ShadowingSession?> GetDetailByIdAsync(
		ShadowingSessionId id,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingSession> Items, int TotalCount)> GetPagedAsync(
		ShadowingSessionFilter filter,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
}
