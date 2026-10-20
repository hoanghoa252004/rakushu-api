using Microsoft.EntityFrameworkCore;
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
			.Include(u => u.Elements)
					.ThenInclude(e => e.JapanesePartOfSpeech)
				.Include(u => u.Elements)
					.ThenInclude(e => e.DependencyRelationship)
			.ToListAsync(cancellationToken);
	}
}