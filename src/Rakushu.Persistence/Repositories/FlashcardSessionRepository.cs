using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Persistence.Repositories;

public sealed class FlashcardSessionRepository : BaseRepository<FlashcardSession, FlashcardSessionId>, IFlashcardSessionRepository
{
	public FlashcardSessionRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<FlashcardSession?> GetByIdWithItemsAsync(
		FlashcardSessionId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Cards)
				.ThenInclude(t => t.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.SingleOrDefaultAsync(fs => fs.Id == id, cancellationToken);
	}

	public async Task<FlashcardSession?> GetActiveSessionByUserAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Cards)
				.ThenInclude(t => t.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.FirstOrDefaultAsync(
				fs => fs.UserId == userId && fs.Status == FlashcardSessionStatus.InProgress,
				cancellationToken);
	}

	public async Task<(IReadOnlyList<FlashcardSession> Items, int TotalCount)> GetSessionsByUserAsync(
		UserId userId,
		FlashcardSessionStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.FlashcardSessions
			.Include(fs => fs.Cards)
			.Include(fs => fs.Items)
			.Where(fs => fs.UserId == userId);

		if (status.HasValue)
		{
			query = query.Where(fs => fs.Status == status.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(fs => fs.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}

	public async Task<FlashcardSession?> GetResultByIdAsync(
		FlashcardSessionId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Cards)
				.ThenInclude(t => t.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
					.ThenInclude(si => si.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
					.ThenInclude(si => si.Schedule)
						.ThenInclude(s => s!.Details)
			.SingleOrDefaultAsync(fs => fs.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<FlashcardSession>> GetStaleSessionsAsync(
		DateTimeOffset now,
		int maxCount,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Cards)
			.Include(fs => fs.Items)
			.Where(fs => fs.Status == FlashcardSessionStatus.InProgress && fs.ExpiresAt < now)
			.OrderBy(fs => fs.ExpiresAt)
			.Take(maxCount)
			.ToListAsync(cancellationToken);
	}

	public async Task<FlashcardRatingSummary> GetRatingSummaryByUserAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		var counts = await _context.FlashcardSessionItems
			.AsNoTracking()
			.Where(item =>
				item.FlashcardSession.UserId == userId &&
				item.FlashcardSession.Status != FlashcardSessionStatus.Canceled)
			.GroupBy(item => item.Rating)
			.Select(group => new
			{
				Rating = group.Key,
				Count = group.Count()
			})
			.ToListAsync(cancellationToken);

		int easy = 0;
		int hard = 0;
		int forgot = 0;

		foreach (var row in counts)
		{
			switch (row.Rating)
			{
				case FlashcardRating.Easy:
					easy = row.Count;
					break;
				case FlashcardRating.Hard:
					hard = row.Count;
					break;
				case FlashcardRating.Forgot:
					forgot = row.Count;
					break;
			}
		}

		return new FlashcardRatingSummary(easy, hard, forgot);
	}
}
