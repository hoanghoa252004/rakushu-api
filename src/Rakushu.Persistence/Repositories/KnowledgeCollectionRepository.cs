using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class KnowledgeCollectionRepository : BaseRepository<KnowledgeCollection, KnowledgeCollectionId>, IKnowledgeCollectionRepository
{
	public KnowledgeCollectionRepository(RakushuDbContext context) : base(context)
	{
	}

	public override async Task<KnowledgeCollection?> GetByIdAsync(KnowledgeCollectionId id, CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeCollections
			.Include(kc => kc.ContentKnowledgeCollections)
			.SingleOrDefaultAsync(kc => kc.Id == id, cancellationToken);
	}

	public async Task<KnowledgeCollection?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeCollections
			.SingleOrDefaultAsync(kc => kc.Code == code, cancellationToken);
	}

	public override async Task<IEnumerable<KnowledgeCollection>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeCollections
			.Include(kc => kc.KnowledgeUnits)
				.ThenInclude(cccccc => cccccc.Elements)
			.ToListAsync(cancellationToken);
	}
}