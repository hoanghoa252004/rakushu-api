using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public interface ISavedItemRepository : IBaseRepository<SavedItem, SavedItemId>
{
	Task<SavedItem?> GetByIdWithScheduleAsync(SavedItemId id, CancellationToken cancellationToken = default);

	Task<SavedItem?> GetByUserAndMeaningAsync(UserId userId, KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(
		UserId userId,
		SrsStatus? statusFilter,
		bool dueOnly,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<int> CountUserItemsAsync(
		UserId userId,
		SrsStatus? statusFilter,
		bool dueOnly,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<SavedItem>> GetDueItemsAsync(
		UserId userId,
		DateTimeOffset now,
		int limit,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<SavedItem>> GetByIdsWithScheduleAsync(
		IEnumerable<SavedItemId> ids,
		UserId userId,
		CancellationToken cancellationToken = default);
}
