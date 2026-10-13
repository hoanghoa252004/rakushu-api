using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;

namespace Rakushu.Persistence.Repositories;

public sealed class ContentCategoryRepository : BaseRepository<ContentCategory, ContentCategoryId>, IContentCategoryRepository
{
	public ContentCategoryRepository(RakushuDbContext context) : base(context) { }

	public async Task<ContentCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
	}

	public override async Task<ContentCategory?> GetByIdAsync(ContentCategoryId id, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Include(c => c.Videos)
			.Include(c => c.Interests)
			.Include(c => c.ContentKnowledgeCollections)
			.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
	}
	public async Task<ContentCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
	}
}
