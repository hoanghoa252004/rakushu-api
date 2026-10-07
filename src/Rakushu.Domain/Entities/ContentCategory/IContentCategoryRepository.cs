using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.ContentCategory;

public interface IContentCategoryRepository : IBaseRepository<ContentCategory, ContentCategoryId>
{
	Task<ContentCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
	Task<ContentCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
