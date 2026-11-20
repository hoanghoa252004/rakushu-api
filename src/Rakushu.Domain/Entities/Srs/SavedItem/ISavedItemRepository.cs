using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public interface ISavedItemRepository : IBaseRepository<SavedItem, SavedItemId>
{
	Task<SavedItem?> GetByIdWithScheduleAsync(SavedItemId id, CancellationToken cancellationToken = default);

	Task<SavedItem?> GetByUserAndMeaningAsync(UserId userId, KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(
		UserId userId,
		int page,
		int pageSize,
		string? filter = null,
		string? sortBy = null,
		string? sortOrder = null,
		CancellationToken cancellationToken = default);

	Task<int> CountUserItemsAsync(
		UserId userId,
		string? filter = null,
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

	Task<SrsDashboardMetrics> GetDashboardMetricsAsync(
		UserId userId,
		DateTimeOffset now,
		CancellationToken cancellationToken = default);

	Task<bool> HasAnyWithMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<SavedItem>> GetByUserAndMeaningsWithScheduleAsync(
		UserId userId,
		IReadOnlyCollection<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default);
}
