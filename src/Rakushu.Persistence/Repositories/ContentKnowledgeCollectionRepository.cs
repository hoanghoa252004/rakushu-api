using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class ContentKnowledgeCollectionRepository : BaseRepository<ContentKnowledgeCollection, ContentKnowledgeCollectionId>, IContentKnowledgeCollectionRepository
{
	public ContentKnowledgeCollectionRepository(RakushuDbContext context) : base(context)
	{
	}

	public override async Task<ContentKnowledgeCollection?> GetByIdAsync(ContentKnowledgeCollectionId id, CancellationToken cancellationToken = default)
	{
		return await _context.ContentKnowledgeCollections
			.Include(x => x.ContentCategory)
			.Include(x => x.KnowledgeCollection)
			.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public override async Task<IEnumerable<ContentKnowledgeCollection>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.ContentKnowledgeCollections
			.Include(x => x.ContentCategory)
			.Include(x => x.KnowledgeCollection)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ContentKnowledgeCollection>> GetByContentCategoryIdAsync(
		ContentCategoryId contentCategoryId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ContentKnowledgeCollections
			.Where(x => x.ContentCategoryId == contentCategoryId)
			.OrderBy(x => x.Priority)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ContentKnowledgeCollection>>
	GetByContentCategoryIdOrderedByPriorityAsync(
		ContentCategoryId contentCategoryId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ContentKnowledgeCollections
			.AsNoTracking()
			.Where(x =>
				x.ContentCategoryId == contentCategoryId &&
				x.IsActive)
			.OrderBy(x => x.Priority)
			.ThenBy(x => x.Id)
			.ToListAsync(cancellationToken);
	}

}