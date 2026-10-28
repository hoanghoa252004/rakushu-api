using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Persistence.Repositories;

public sealed class SavedItemRepository : BaseRepository<SavedItem, SavedItemId>, ISavedItemRepository
{
	public SavedItemRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<SavedItem?> GetByIdWithScheduleAsync(
		SavedItemId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
				.ThenInclude(s => s!.Details)
			.Include(si => si.KnowledgeMeaning)
			.SingleOrDefaultAsync(si => si.Id == id, cancellationToken);
	}

	public async Task<SavedItem?> GetByUserAndMeaningAsync(
		UserId userId,
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
			.Include(si => si.KnowledgeMeaning)
			.SingleOrDefaultAsync(
				si => si.UserId == userId && si.KnowledgeMeaningId == meaningId,
				cancellationToken);
	}

	public async Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(
		UserId userId,
		SrsStatus? statusFilter,
		bool dueOnly,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = BuildUserItemsQuery(userId, statusFilter, dueOnly);

		return await query
			.OrderBy(si => si.Schedule != null ? si.Schedule.DueAt : si.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<int> CountUserItemsAsync(
		UserId userId,
		SrsStatus? statusFilter,
		bool dueOnly,
		CancellationToken cancellationToken = default)
	{
		var query = BuildUserItemsQuery(userId, statusFilter, dueOnly);
		return await query.CountAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<SavedItem>> GetDueItemsAsync(
		UserId userId,
		DateTimeOffset now,
		int limit,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
			.Include(si => si.KnowledgeMeaning)
			.Where(si => si.UserId == userId
				&& si.Schedule != null
				&& si.Schedule.DueAt <= now
				&& si.Schedule.Status != SrsStatus.Suspended
				&& si.Schedule.Status != SrsStatus.Ignored)
			.OrderBy(si => si.Schedule!.DueAt)
			.Take(limit)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<SavedItem>> GetByIdsWithScheduleAsync(
		IEnumerable<SavedItemId> ids,
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		var idList = ids.ToList();
		return await _context.SavedItems
			.Include(si => si.Schedule)
				.ThenInclude(s => s!.Details)
			.Include(si => si.KnowledgeMeaning)
			.Where(si => si.UserId == userId && idList.Contains(si.Id))
			.ToListAsync(cancellationToken);
	}

	private IQueryable<SavedItem> BuildUserItemsQuery(
		UserId userId,
		SrsStatus? statusFilter,
		bool dueOnly)
	{
		var query = _context.SavedItems
			.Include(si => si.Schedule)
			.Include(si => si.KnowledgeMeaning)
			.Where(si => si.UserId == userId);

		var now = DateTimeOffset.UtcNow;
		var todayStart = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
		var tomorrowStart = todayStart.AddDays(1);

		if (dueOnly || statusFilter == SrsStatus.Due_today)
		{
			query = query.Where(si => si.Schedule != null
				&& si.Schedule.DueAt <= now
				&& si.Schedule.Status != SrsStatus.Suspended
				&& si.Schedule.Status != SrsStatus.Ignored);
		}
		else if (statusFilter == SrsStatus.Overdue)
		{
			query = query.Where(si => si.Schedule != null
				&& si.Schedule.DueAt < todayStart
				&& si.Schedule.Status != SrsStatus.Suspended
				&& si.Schedule.Status != SrsStatus.Ignored);
		}
		else if (statusFilter == SrsStatus.Suspended)
		{
			query = query.Where(si => si.Schedule != null && si.Schedule.Status == SrsStatus.Suspended);
		}
		else if (statusFilter == SrsStatus.Learning)
		{
			query = query.Where(si => si.Schedule != null
				&& si.Schedule.Status == SrsStatus.Learning
				&& si.Schedule.DueAt >= tomorrowStart);
		}
		else if (statusFilter == SrsStatus.Review)
		{
			query = query.Where(si => si.Schedule != null
				&& si.Schedule.Status == SrsStatus.Review
				&& si.Schedule.DueAt >= tomorrowStart);
		}
		else
		{
			// All: exclude Ignored
			query = query.Where(si => si.Schedule == null || si.Schedule.Status != SrsStatus.Ignored);
		}

		return query;
	}
}
