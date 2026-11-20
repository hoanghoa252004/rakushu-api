using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

namespace Rakushu.Persistence.Repositories;

internal sealed class SavedItemRepository : BaseRepository<SavedItem, SavedItemId>, ISavedItemRepository
{
	public SavedItemRepository(RakushuDbContext context) : base(context) { }

	public override async Task<SavedItem?> GetByIdAsync(
		SavedItemId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
	}

	public async Task<SavedItem?> GetByIdWithScheduleAsync(
		SavedItemId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
				.ThenInclude(sch => sch!.Details)
					.ThenInclude(d => d.QuizAttemptItem)
			.Include(si => si.Schedule)
				.ThenInclude(sch => sch!.Details)
					.ThenInclude(d => d.FlashcardSessionItem)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
	}

	public async Task<SavedItem?> GetByUserAndMeaningAsync(
		UserId userId,
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.Include(si => si.Schedule)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.FirstOrDefaultAsync(
				si => si.UserId == userId && si.KnowledgeMeaningId == meaningId,
				cancellationToken);
	}

	public async Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(
		UserId userId,
		int page,
		int pageSize,
		string? filter = null,
		string? sortBy = null,
		string? sortOrder = null,
		CancellationToken cancellationToken = default)
	{
		var query = BuildUserItemsQuery(userId, filter);

		query = ApplySorting(query, sortBy, sortOrder);

		return await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	private static IQueryable<SavedItem> ApplySorting(
		IQueryable<SavedItem> query,
		string? sortBy,
		string? sortOrder)
	{
		var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();
		var isDesc = string.Equals(sortOrder?.Trim(), "desc", StringComparison.OrdinalIgnoreCase);
		var isAsc = string.Equals(sortOrder?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);

		return normalizedSortBy switch
		{
			// 1. Mới lưu nhất (mặc định DESC: mới nhất lên đầu)
			"saved" or "savedat" or "newest" or "recent" =>
				isAsc
					? query.OrderBy(si => si.SavedAt)
					: query.OrderByDescending(si => si.SavedAt),

			// 2. Cần ôn (mặc định ASC: đến hạn/quá hạn sớm nhất lên đầu)
			"due" or "dueat" or "urgent" or "urgency" =>
				isDesc
					? query.OrderByDescending(si => si.Schedule != null ? si.Schedule.DueAt : si.SavedAt)
					: query.OrderBy(si => si.Schedule != null ? si.Schedule.DueAt : si.SavedAt),

			// 3. Từ hay quên (mặc định DESC: số lần quên tích lũy Lapses nhiều nhất lên đầu)
			"leech" or "forgotten" or "lapses" or "difficulty" =>
				isAsc
					? query
						.OrderBy(si => si.Schedule != null
							? si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (int?)d.Lapses).FirstOrDefault() ?? 0
							: 0)
						.ThenByDescending(si => si.Schedule != null
							? si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (double?)d.EasinessFactor).FirstOrDefault() ?? 2.5
							: 2.5)
					: query
						.OrderByDescending(si => si.Schedule != null
							? si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (int?)d.Lapses).FirstOrDefault() ?? 0
							: 0)
						.ThenBy(si => si.Schedule != null
							? si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (double?)d.EasinessFactor).FirstOrDefault() ?? 2.5
							: 2.5),

			// 4. Bảng chữ cái (mặc định ASC: A->Z / あ->ん theo Reading rồi tới Expression)
			"alphabetical" or "alpha" or "expression" or "reading" or "word" or "kana" =>
				isDesc
					? query
						.OrderByDescending(si => si.KnowledgeMeaning.KnowledgeUnit.Reading)
						.ThenByDescending(si => si.KnowledgeMeaning.KnowledgeUnit.Expression)
					: query
						.OrderBy(si => si.KnowledgeMeaning.KnowledgeUnit.Reading)
						.ThenBy(si => si.KnowledgeMeaning.KnowledgeUnit.Expression),

			// Mặc định: theo lịch ôn (DueAt ASC / SavedAt ASC)
			_ => isDesc
				? query.OrderByDescending(si => si.Schedule != null ? si.Schedule.DueAt : si.SavedAt)
				: query.OrderBy(si => si.Schedule != null ? si.Schedule.DueAt : si.SavedAt)
		};
	}

	public async Task<int> CountUserItemsAsync(
		UserId userId,
		string? filter = null,
		CancellationToken cancellationToken = default)
	{
		var query = BuildUserItemsQuery(userId, filter);
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
				.ThenInclude(km => km.KnowledgeUnit)
			.Where(si => si.UserId == userId
				&& si.IsActive
				&& si.Schedule != null
				&& si.Schedule.DueAt <= now)
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
				.ThenInclude(sch => sch!.Details)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.Where(si => idList.Contains(si.Id) && si.UserId == userId && si.IsActive)
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
			.Where(si => si.UserId == userId && si.IsActive)
			.Select(si => new
			{
				HasSchedule = si.Schedule != null,
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

			if (item.LatestDetail != null)
			{
				if (item.LatestDetail.EasinessFactor <= 1.3001 || item.LatestDetail.Lapses >= 8)
				{
					leechCount++;
				}

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
		var recentLogs = await _context.Set<SrsScheduleDetail>()
			.AsNoTracking()
			.Where(d => d.ReviewedAt >= thirtyDaysAgo
				&& d.SrsSchedule.SavedItem.UserId == userId
				&& d.SrsSchedule.SavedItem.IsActive)
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
		string? filter)
	{
		var query = _context.SavedItems
			.Include(si => si.Schedule)
				.ThenInclude(sch => sch!.Details)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.Where(si => si.UserId == userId);

		var normalizedFilter = filter?.Trim().ToLowerInvariant();

		var now = DateTimeOffset.UtcNow;
		var endOfToday = new DateTimeOffset(now.Year, now.Month, now.Day, 23, 59, 59, 999, now.Offset);

		return normalizedFilter switch
		{
			// 1. Cần ôn hôm nay & Quá hạn (Due today / Overdue)
			"due" or "due_today" or "overdue" or "urgent" =>
				query.Where(si => si.IsActive && si.Schedule != null && si.Schedule.DueAt <= endOfToday),

			// 2. Từ hay quên (Leech: Lapses >= 8 hoặc EF <= 1.3001)
			"leech" or "hay_quen" or "difficult" =>
				query.Where(si => si.IsActive && si.Schedule != null &&
					(si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (int?)d.Lapses).FirstOrDefault() >= 8 ||
					 si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (double?)d.EasinessFactor).FirstOrDefault() <= 1.3001)),

			// 3. Đã thành thạo (Mastered: Interval >= 21 ngày)
			"mastered" or "thanh_thao" or "solidified" =>
				query.Where(si => si.IsActive && si.Schedule != null &&
					si.Schedule.Details.OrderByDescending(d => d.ReviewNumber).Select(d => (int?)d.IntervalDays).FirstOrDefault() >= 21),

			// 4. Mặc định: Lấy toàn bộ từ vựng đang học (Active)
			_ => query.Where(si => si.IsActive)
		};
	}

	public async Task<bool> HasAnyWithMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.SavedItems
			.AnyAsync(si => si.KnowledgeMeaningId == meaningId, cancellationToken);
	}

	public async Task<IReadOnlyList<SavedItem>> GetByUserAndMeaningsWithScheduleAsync(
		UserId userId,
		IReadOnlyCollection<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default)
	{
		if (meaningIds.Count == 0)
		{
			return [];
		}

		return await _context.SavedItems
			.Include(si => si.Schedule)
				.ThenInclude(sch => sch!.Details)
			.Include(si => si.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.Where(si => si.UserId == userId && si.IsActive && meaningIds.Contains(si.KnowledgeMeaningId))
			.ToListAsync(cancellationToken);
	}
}