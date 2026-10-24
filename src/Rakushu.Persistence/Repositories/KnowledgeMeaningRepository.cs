using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
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

	public override async Task<KnowledgeMeaning?> GetByIdAsync(KnowledgeMeaningId id, CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeMeanings
			.Include(m => m.LearningUnits)
			.Include(m => m.KnowledgeUnit)
			.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<KnowledgeMeaning>> GetByKnowledgeUnitIdAsync(
		KnowledgeUnitId knowledgeUnitId,
		CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeMeanings
			.Where(m => m.KnowledgeUnitId == knowledgeUnitId)
			.OrderBy(m => m.SortOrder)
			.ToListAsync(cancellationToken);
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
