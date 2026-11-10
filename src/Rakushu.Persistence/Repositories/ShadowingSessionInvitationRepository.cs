using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingSessionInvitationRepository
	: BaseRepository<ShadowingSessionInvitation, ShadowingSessionInvitationId>, IShadowingSessionInvitationRepository
{
	public ShadowingSessionInvitationRepository(RakushuDbContext context) : base(context) { }

	public async Task<bool> PendingExistsAsync(
		ShadowingSessionId sessionId, UserId learnerId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionInvitations.AnyAsync(
			i => i.ShadowingSessionId == sessionId
				&& i.LearnerId == learnerId
				&& i.Status == SessionInvitationStatus.Pending,
			cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingSessionInvitation>> GetPendingBySessionAsync(
		ShadowingSessionId sessionId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionInvitations
			.Where(i => i.ShadowingSessionId == sessionId && i.Status == SessionInvitationStatus.Pending)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingSessionInvitation>> GetExpiredPendingAsync(
		DateTimeOffset now, int batchSize, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionInvitations
			.Where(i => i.Status == SessionInvitationStatus.Pending && i.ExpiredAt <= now)
			.OrderBy(i => i.ExpiredAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingSessionInvitation> Items, int TotalCount)> GetPagedByLearnerAsync(
		UserId learnerId,
		SessionInvitationStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingSessionInvitations.AsNoTracking()
			.Where(i => i.LearnerId == learnerId);
		if (status.HasValue)
			query = query.Where(i => i.Status == status.Value);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(i => i.SentAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
		return (items, total);
	}
}
