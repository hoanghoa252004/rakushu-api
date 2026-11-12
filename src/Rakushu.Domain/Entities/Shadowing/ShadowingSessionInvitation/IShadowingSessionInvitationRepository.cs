using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

public interface IShadowingSessionInvitationRepository : IBaseRepository<ShadowingSessionInvitation, ShadowingSessionInvitationId>
{
	Task<bool> PendingExistsAsync(
		ShadowingSessionId sessionId,
		UserId learnerId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ShadowingSessionInvitation>> GetPendingBySessionAsync(
		ShadowingSessionId sessionId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ShadowingSessionInvitation>> GetExpiredPendingAsync(
		DateTimeOffset now,
		int batchSize,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingSessionInvitation> Items, int TotalCount)> GetPagedByLearnerAsync(
		UserId learnerId,
		SessionInvitationStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
}
