using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class KnowledgeUnitRepository : BaseRepository<KnowledgeUnit, KnowledgeUnitId>, IKnowledgeUnitRepository
{
	public KnowledgeUnitRepository(RakushuDbContext context) : base(context)
	{
	}

	public override async Task<KnowledgeUnit?> GetByIdAsync(KnowledgeUnitId id, CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeUnits
			.Include(x => x.ProficiencyLevel)
			.Include(x => x.Meanings)
			.Include(u => u.Elements)
				.ThenInclude(e => e.JapanesePartOfSpeech)
			.Include(u => u.Elements)
				.ThenInclude(e => e.DependencyRelationship)
			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<KnowledgeUnit>> GetPagedAsync(
	int page,
	int pageSize,
	CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeUnits
				.Include(u => u.Elements)
					.ThenInclude(e => e.JapanesePartOfSpeech)
				.Include(u => u.Elements)
					.ThenInclude(e => e.DependencyRelationship)
				.OrderBy(x => x.Id)
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);
	}

	public override async Task<IEnumerable<KnowledgeUnit>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.KnowledgeUnits
			.Include(u => u.ProficiencyLevel)
			.Include(u => u.Elements)
					.ThenInclude(e => e.JapanesePartOfSpeech)
				.Include(u => u.Elements)
					.ThenInclude(e => e.DependencyRelationship)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<KnowledgeUnit>> GetActiveByCollectionIdsAsync(
	IReadOnlyCollection<KnowledgeCollectionId> collectionIds,
	CancellationToken cancellationToken = default)
	{
		if (collectionIds.Count == 0)
			return Array.Empty<KnowledgeUnit>();

		return await _context.KnowledgeUnits
			//.AsNoTracking()
			.Include(x => x.Meanings)
			.Where(x =>
				x.IsActive &&
				collectionIds.Contains(x.KnowledgeCollectionId))
			.Include(x => x.Elements)
				.ThenInclude(e => e.JapanesePartOfSpeech)
			.Include(x => x.Elements)
				.ThenInclude(e => e.DependencyRelationship)
			.OrderBy(x => x.Id)
			.ToListAsync(cancellationToken);
	}
}