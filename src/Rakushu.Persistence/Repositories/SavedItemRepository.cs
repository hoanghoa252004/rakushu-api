using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;
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

	public async Task<SrsDashboardMetrics> GetDashboardMetricsAsync(
		UserId userId,
		DateTimeOffset now,
		CancellationToken cancellationToken = default)
	{
		var today = now.Date;
		var thirtyDaysAgo = now.AddDays(-30);

		// 1. Load active saved items for this user with schedule and latest detail
		var activeItems = await _context.SavedItems
			.AsNoTracking()
			.Where(si => si.UserId == userId && (si.Schedule == null || (si.Schedule.Status != SrsStatus.Suspended && si.Schedule.Status != SrsStatus.Ignored)))
			.Select(si => new
			{
				HasSchedule = si.Schedule != null,
				Status = si.Schedule != null ? si.Schedule.Status : SrsStatus.Learning,
				DueAt = si.Schedule != null ? (DateTimeOffset?)si.Schedule.DueAt : null,
				LatestDetail = si.Schedule != null ? si.Schedule.Details
					.OrderByDescending(d => d.ReviewNumber)
					.Select(d => new
					{
						d.IntervalDays,
						d.EasinessFactor,
						d.Lapses,
						d.Repetitions
					})
					.FirstOrDefault() : null
			})
			.ToListAsync(cancellationToken);

		int totalActiveWords = activeItems.Count;
		int dueTodayCount = 0;
		int overdueCount = 0;
		int leechCount = 0;
		int masteredWordsCount = 0;

		foreach (var item in activeItems)
		{
			// Check Due / Overdue
			if (item.DueAt.HasValue)
			{
				var dueDate = item.DueAt.Value.Date;
				if (dueDate < today)
				{
					overdueCount++;
				}
				else if (dueDate == today || item.DueAt.Value <= now)
				{
					dueTodayCount++;
				}
			}

			// Check Leech: EF <= 1.3001 OR Lapses >= 8
			if (item.LatestDetail != null)
			{
				if (item.LatestDetail.EasinessFactor <= 1.3001 || item.LatestDetail.Lapses >= 8)
				{
					leechCount++;
				}

				// Check Mastered: Interval >= 21 days
				if (item.LatestDetail.IntervalDays >= 21)
				{
					masteredWordsCount++;
				}
			}
		}

		double masteryRate = totalActiveWords > 0
			? Math.Round((double)masteredWordsCount / totalActiveWords * 100.0, 1)
			: 0.0;

		// 2. Query 30-day review logs for True Retention Rate
		var recentLogs = await _context.Set<SrsItemDetail>()
			.AsNoTracking()
			.Where(d => d.ReviewedAt >= thirtyDaysAgo
				&& d.SrsSchedule.SavedItem.UserId == userId
				&& (d.SrsSchedule.Status == SrsStatus.Learning || d.SrsSchedule.Status == SrsStatus.Review))
			.Select(d => d.Quality)
			.ToListAsync(cancellationToken);

		int totalReviews30Days = recentLogs.Count;
		int successfulReviews30Days = recentLogs.Count(q => q >= 3);
		double trueRetentionRate = totalReviews30Days > 0
			? Math.Round((double)successfulReviews30Days / totalReviews30Days * 100.0, 1)
			: 0.0;

		int totalDueCount = overdueCount + dueTodayCount;

		return new SrsDashboardMetrics(
			TotalDueCount: totalDueCount,
			DueTodayCount: dueTodayCount,
			OverdueCount: overdueCount,
			TotalActiveWords: totalActiveWords,
			TrueRetentionRate: trueRetentionRate,
			SuccessfulReviewsLast30Days: successfulReviews30Days,
			TotalReviewsLast30Days: totalReviews30Days,
			LeechCount: leechCount,
			MasteryRate: masteryRate,
			MasteredWordsCount: masteredWordsCount);
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
