using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Persistence.Repositories;

public sealed class ContentCategoryRepository : BaseRepository<ContentCategory, ContentCategoryId>, IContentCategoryRepository
{
	public ContentCategoryRepository(RakushuDbContext context) : base(context) { }

	public async Task<ContentCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Include(c => c.Children)
			.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
	}

	public override async Task<ContentCategory?> GetByIdAsync(ContentCategoryId id, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Include(c => c.Children)
			.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
	}
}
