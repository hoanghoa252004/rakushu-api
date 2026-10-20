using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Subscriptions.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;
public sealed class KnowledgeMeaningRepository : BaseRepository<KnowledgeMeaning, KnowledgeMeaningId>, IKnowledgeMeaningRepository
{
	public KnowledgeMeaningRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IReadOnlyList<KnowledgeMeaning>>
		GetActiveMeaningsForEmbeddingAsync(
			int offset,
			int batchSize,
			CancellationToken cancellationToken = default)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(offset));
		}

		if (batchSize <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(batchSize));
		}

		return await _context.KnowledgeMeanings
			.AsNoTracking()
			.Where(x =>
				x.KnowledgeUnit.IsActive &&
				x.KnowledgeUnit.KnowledgeCollection.IsActive)
			.Include(x => x.KnowledgeUnit)
				.ThenInclude(x => x.KnowledgeCollection)
			.OrderBy(x => x.KnowledgeUnitId)
			.ThenBy(x => x.SortOrder)
			.Skip(offset)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}
}
