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

	public async Task<IEnumerable<ContentCategory>> GetByParentIdAsync(ContentCategoryId parentId, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Where(c => c.ParentId == parentId)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> ExistsDisplayOrderInLevelAsync(int level, int displayOrder, ContentCategoryId? excludeId = null, CancellationToken cancellationToken = default)
	{
		var query = _context.ContentCategories
			.Where(c => c.Level == level && c.DisplayOrder == displayOrder);

		if (excludeId is not null)
		{
			query = query.Where(c => c.Id != excludeId);
		}

		return await query.AnyAsync(cancellationToken);
	}

	public async Task<IEnumerable<ContentCategory>> GetActiveAsync(CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Where(c => c.Status == ContentCategoryStatus.Active)
			.Include(c => c.Children)
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<ContentCategory>> GetByStatusAsync(ContentCategoryStatus status, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Where(c => c.Status == status)
			.Include(c => c.Children)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> HasRelatedContentAsync(ContentCategoryId id, CancellationToken cancellationToken = default)
	{
		// Check if category has any related content in:
		// 1. ContentProcessingPolicies
		// 2. Videos
		// 3. Series

		var hasRelatedContent = await _context.ContentCategories
			.Where(c => c.Id == id)
			.Select(c => 
				c.ContentProcessingPolicies.Any() ||
				c.Videos.Any() ||
				c.Series.Any())
			.FirstOrDefaultAsync(cancellationToken);

		return hasRelatedContent;
	}

	public override async Task<ContentCategory?> GetByIdAsync(ContentCategoryId id, CancellationToken cancellationToken = default)
	{
		return await _context.ContentCategories
			.Include(c => c.Children)
			.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
	}
}
