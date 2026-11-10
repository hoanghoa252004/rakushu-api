using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingAttemptRepository
	: BaseRepository<ShadowingAttempt, ShadowingAttemptId>, IShadowingAttemptRepository
{
	public ShadowingAttemptRepository(RakushuDbContext context) : base(context) { }

	public async Task<ShadowingAttempt?> GetWithContextAsync(
		ShadowingAttemptId id, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttempts
			.Include(a => a.ShadowingSession)
			.Include(a => a.ShadowingSessionMember)
			.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
	}

	public async Task<ShadowingAttempt?> GetByMemberAsync(
		ShadowingSessionId sessionId, UserId userId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttempts
			.Include(a => a.ShadowingSession)
			.Include(a => a.ShadowingSessionMember)
			.FirstOrDefaultAsync(
				a => a.ShadowingSessionId == sessionId && a.ShadowingSessionMember.MemberId == userId,
				cancellationToken);
	}

	public async Task<ShadowingAttemptDetail?> GetDetailAsync(
		ShadowingAttemptId attemptId, ShadowingRoleLineId roleLineId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.FirstOrDefaultAsync(
			d => d.ShadowingAttemptId == attemptId && d.ShadowingRoleLineId == roleLineId, cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingAttemptDetail>> GetDetailsAsync(
		ShadowingAttemptId attemptId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.AsNoTracking()
			.Where(d => d.ShadowingAttemptId == attemptId)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> RoleLineDoneInSessionAsync(
		ShadowingSessionId sessionId, ShadowingRoleLineId roleLineId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.AnyAsync(
			d => d.ShadowingRoleLineId == roleLineId && d.ShadowingAttempt.ShadowingSessionId == sessionId,
			cancellationToken);
	}

	public async Task<int> CountSkippedDetailsAsync(
		ShadowingAttemptId attemptId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.CountAsync(
			d => d.ShadowingAttemptId == attemptId && d.IsSkipped, cancellationToken);
	}

	public async Task<int> CountDetailsAsync(
		ShadowingAttemptId attemptId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.CountAsync(
			d => d.ShadowingAttemptId == attemptId, cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingAttemptDetail> Items, int TotalCount)> GetDetailsPagedAsync(
		ShadowingAttemptId attemptId, int pageNumber, int pageSize, CancellationToken cancellationToken = default, bool? isSkipped = null)
	{
		var query = _context.ShadowingAttemptDetails.AsNoTracking()
			.Where(d => d.ShadowingAttemptId == attemptId);
		if (isSkipped.HasValue)
			query = query.Where(d => d.IsSkipped == isSkipped.Value);
		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderBy(d => d.SubmittedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
		return (items, total);
	}

	public async Task<bool> IsRoleAttemptCancelledAsync(
		ShadowingSessionId sessionId, Rakushu.Domain.Entities.Shadowing.ShadowingRole.ShadowingRoleId roleId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttempts.AnyAsync(
			a => a.ShadowingSessionId == sessionId
				&& a.ShadowingSessionMember.ShadowingRoleId == roleId
				&& a.Status == ShadowingAttemptStatus.Cancelled,
			cancellationToken);
	}

	public void AddDetail(ShadowingAttemptDetail detail)
	{
		_context.ShadowingAttemptDetails.Add(detail);
	}

	public async Task<IReadOnlyList<ShadowingAttempt>> GetBySessionAsync(
		ShadowingSessionId sessionId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttempts.AsNoTracking()
			.Include(a => a.ShadowingSessionMember)
			.Where(a => a.ShadowingSessionId == sessionId)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingAttemptDetail>> GetSessionDetailsAsync(
		ShadowingSessionId sessionId,
		IReadOnlyCollection<ShadowingRoleLineId> roleLineIds,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingAttemptDetails.AsNoTracking()
			.Include(d => d.ShadowingAttempt)
			.Where(d => d.ShadowingAttempt.ShadowingSessionId == sessionId && roleLineIds.Contains(d.ShadowingRoleLineId))
			.ToListAsync(cancellationToken);
	}
}
