using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class SrsItemRepository : BaseRepository<SrsItem, SrsItemId>, ISrsItemRepository
{
	public SrsItemRepository(RakushuDbContext context) : base(context) { }

	public async Task<IReadOnlyList<SrsItem>> GetItemsAsync(
		UserId userId, CancellationToken cancellationToken = default)
	{
		return await QueryQuizItems(userId, DateTimeOffset.MinValue, false)
			.AsNoTracking()
			.OrderBy(item => item.DueAt)
			.ThenBy(item => item.CreatedAt)
			.ToListAsync(cancellationToken);
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
