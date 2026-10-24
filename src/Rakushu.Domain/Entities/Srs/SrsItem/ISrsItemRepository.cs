using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface ISrsItemRepository : IBaseRepository<SrsItem, SrsItemId>
{
	Task<SrsItem?> GetByIdForUserAsync(SrsItemId itemId, UserId userId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<SrsItem>> GetItemsAsync(UserId userId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<SrsItem>> GetItemsAsync(UserId userId, SrsItemFilter filter, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<SrsItem>> GetQuizItemsByIdsAsync(UserId userId, IReadOnlyCollection<Guid> itemIds, DateTimeOffset now, bool dueOnly, CancellationToken cancellationToken = default);
	Task<bool> HasQuizHistoryAsync(SrsItemId itemId, CancellationToken cancellationToken = default);
}
