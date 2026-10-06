using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.ContentCategory;

public interface IContentCategoryRepository : IBaseRepository<ContentCategory, ContentCategoryId>
{
	Task<ContentCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

	Task<IEnumerable<ContentCategory>> GetByParentIdAsync(ContentCategoryId parentId, CancellationToken cancellationToken = default);

	Task<bool> ExistsDisplayOrderInLevelAsync(int level, int displayOrder, ContentCategoryId? excludeId = null, CancellationToken cancellationToken = default);

	Task<IEnumerable<ContentCategory>> GetActiveAsync(CancellationToken cancellationToken = default);

	Task<IEnumerable<ContentCategory>> GetByStatusAsync(ContentCategoryStatus status, CancellationToken cancellationToken = default);

	Task<bool> HasRelatedContentAsync(ContentCategoryId id, CancellationToken cancellationToken = default);
}
