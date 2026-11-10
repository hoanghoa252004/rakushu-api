using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingSessionRepository
	: BaseRepository<ShadowingSession, ShadowingSessionId>, IShadowingSessionRepository
{
	public ShadowingSessionRepository(RakushuDbContext context) : base(context) { }

	public override async Task<ShadowingSession?> GetByIdAsync(
		ShadowingSessionId id, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions
			.Include(s => s.Members)
			.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingSession>> GetExpiredScheduledAsync(
		DateTimeOffset now, int batchSize, CancellationToken cancellationToken = default)
	{
		var threshold = now - ShadowingSession.ExpirationGracePeriod;
		return await _context.ShadowingSessions
			.Where(s => s.Status == ShadowingSessionStatus.Scheduled && s.ScheduledStartAt <= threshold)
			.OrderBy(s => s.ScheduledStartAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> HasActiveHostedSessionAsync(
		UserId hostId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions.AnyAsync(
			s => s.HostId == hostId
				&& s.Status != ShadowingSessionStatus.Completed
				&& s.Status != ShadowingSessionStatus.Cancelled
				&& s.Status != ShadowingSessionStatus.Expired,
			cancellationToken);
	}

	public async Task<bool> HasScheduleConflictAsync(
		UserId userId,
		DateTimeOffset start,
		TimeSpan duration,
		ShadowingSessionId? excludeSessionId,
		CancellationToken cancellationToken = default)
	{
		var end = start + duration;
		return await _context.ShadowingSessions.AnyAsync(
			s => s.Members.Any(m => m.MemberId == userId)
				&& (s.Status == ShadowingSessionStatus.Scheduled || s.Status == ShadowingSessionStatus.InProgress)
				&& (excludeSessionId == null || s.Id != excludeSessionId)
				&& s.ScheduledStartAt < end
				&& s.ScheduledStartAt + s.ShadowingMaterial.Video.Duration > start,
			cancellationToken);
	}

	public async Task<TimeSpan?> GetDurationAsync(
		ShadowingSessionId sessionId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions
			.Where(s => s.Id == sessionId)
			.Select(s => (TimeSpan?)s.ShadowingMaterial.Video.Duration)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ShadowingSession?> GetWithAttemptsAsync(
		ShadowingSessionId id, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions
			.Include(s => s.Members)
			.Include(s => s.Attempts)
			.AsSplitQuery()
			.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingSession>> GetInProgressWithAttemptsAsync(
		int batchSize, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions
			.Include(s => s.Attempts)
			.Where(s => s.Status == ShadowingSessionStatus.InProgress
				&& s.Attempts.Any()
				&& s.Attempts.All(a => a.Status != ShadowingAttemptStatus.InProgress))
			.OrderBy(s => s.StartedAt)
			.Take(batchSize)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);
	}

	public async Task<ShadowingSession?> GetDetailByIdAsync(
		ShadowingSessionId id, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions.AsNoTracking()
			.Include(s => s.ShadowingMaterial).ThenInclude(m => m.Video)
			.Include(s => s.ShadowingMaterial).ThenInclude(m => m.Roles)
			.Include(s => s.Members).ThenInclude(m => m.Member)
			.Include(s => s.Members).ThenInclude(m => m.ShadowingRole).ThenInclude(r => r!.Speaker)
			.AsSplitQuery()
			.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingSession> Items, int TotalCount)> GetPagedAsync(
		ShadowingSessionFilter filter,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingSessions.AsNoTracking().AsQueryable();

		if (filter.MemberId is not null)
		{
			var memberId = filter.MemberId;
			query = query.Where(s => s.Members.Any(m => m.MemberId == memberId));
		}
		if (filter.HostId is not null)
		{
			var hostId = filter.HostId;
			query = query.Where(s => s.HostId == hostId);
		}
		if (filter.Status.HasValue)
			query = query.Where(s => s.Status == filter.Status.Value);
		if (filter.Visibility.HasValue)
			query = query.Where(s => s.Visibility == filter.Visibility.Value);
		if (filter.MaterialId is not null)
		{
			var materialId = filter.MaterialId;
			query = query.Where(s => s.ShadowingMaterialId == materialId);
		}
		if (filter.StartsAfter.HasValue)
			query = query.Where(s => s.ScheduledStartAt > filter.StartsAfter.Value);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.Include(s => s.ShadowingMaterial).ThenInclude(m => m.Video)
			.Include(s => s.ShadowingMaterial).ThenInclude(m => m.Roles)
			.Include(s => s.Members).ThenInclude(m => m.Member)
			.Include(s => s.Members).ThenInclude(m => m.ShadowingRole).ThenInclude(r => r!.Speaker)
			.OrderByDescending(s => s.Members.Min(m => m.JoinedAt))
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);

		return (items, total);
	}
}
