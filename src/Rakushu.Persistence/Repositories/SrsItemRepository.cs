using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class SrsItemRepository : BaseRepository<SrsItem, SrsItemId>, ISrsItemRepository
{
	public SrsItemRepository(RakushuDbContext context) : base(context) { }

	public Task<SrsItem?> GetByIdForUserAsync(
		SrsItemId itemId, UserId userId, CancellationToken cancellationToken = default)
	{
		return _context.SrsItems
			.FirstOrDefaultAsync(item => item.Id == itemId && item.UserId == userId, cancellationToken);
	}

	public Task<IReadOnlyList<SrsItem>> GetItemsAsync(
		UserId userId, CancellationToken cancellationToken = default)
	{
		return GetItemsAsync(userId, SrsItemFilter.Active, cancellationToken);
	}

	public async Task<IReadOnlyList<SrsItem>> GetItemsAsync(
		UserId userId, SrsItemFilter filter, CancellationToken cancellationToken = default)
	{
		IQueryable<SrsItem> query = _context.SrsItems
			.AsNoTracking()
			.Where(item => item.UserId == userId);

		query = filter switch
		{
			SrsItemFilter.Suspended => query.Where(item => item.Status == SrsItemStatus.Suspended).OrderByDescending(item => item.UpdatedAt),
			SrsItemFilter.Learning => query.Where(item => item.Status == SrsItemStatus.Learning).OrderBy(item => item.DueAt).ThenBy(item => item.CreatedAt),
			SrsItemFilter.Review => query.Where(item => item.Status == SrsItemStatus.Review).OrderBy(item => item.DueAt).ThenBy(item => item.CreatedAt),
			SrsItemFilter.Ignored => query.Where(item => item.Status == SrsItemStatus.Ignored).OrderByDescending(item => item.UpdatedAt),
			_ => query.Where(item =>
				item.Status != SrsItemStatus.Suspended &&
				item.Status != SrsItemStatus.Ignored)
				.OrderBy(item => item.DueAt)
				.ThenBy(item => item.CreatedAt)
		};

		return await query.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<SrsItem>> GetQuizItemsByIdsAsync(
		UserId userId,
		IReadOnlyCollection<Guid> itemIds,
		DateTimeOffset now,
		bool dueOnly,
		CancellationToken cancellationToken = default)
	{
		if (itemIds.Count == 0)
		{
			return Array.Empty<SrsItem>();
		}

		var typedItemIds = itemIds.Select(SrsItemId.From).ToArray();
		var query = QueryQuizItems(userId, now, dueOnly)
			.Where(item => typedItemIds.Contains(item.Id));

		return await query
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public Task<bool> HasQuizHistoryAsync(
		SrsItemId itemId, CancellationToken cancellationToken = default)
	{
		return _context.QuizAttemptItems
			.AnyAsync(q => q.SrsItemId == itemId, cancellationToken);
	}

	private IQueryable<SrsItem> QueryQuizItems(UserId userId, DateTimeOffset now, bool dueOnly)
	{
		var query = _context.SrsItems
			.Where(item =>
				item.UserId == userId &&
				item.Status != SrsItemStatus.Suspended &&
				item.Status != SrsItemStatus.Ignored);

		if (dueOnly)
		{
			query = query.Where(item => item.DueAt <= now);
		}

		return query;
	}
}
