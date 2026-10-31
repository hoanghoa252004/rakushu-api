using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class LearningUnitRepository : BaseRepository<LearningUnit, LearningUnitId>, ILearningUnitRepository
{
	public LearningUnitRepository(RakushuDbContext context) : base(context) { }

	public async Task<LearningUnit?> GetDetailByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default)
	{
		return await _context.LearningUnits
			//.AsNoTracking()
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.KnowledgeCollection)

			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.ProficiencyLevel)

			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.Meanings)

			.AsSplitQuery()
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
	}

	public async Task<(IReadOnlyList<LearningUnit> Items, int TotalCount)> GetPagedByVideoIdAsync(
		VideoId videoId,
		KnowledgeUnitType? knowledgeType,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.LearningUnits
			//.AsNoTracking()
			.Where(l => l.TranscriptSegment.Transcript.VideoId == videoId);

		if (knowledgeType.HasValue)
			query = query.Where(l => l.KnowledgeMeaning.KnowledgeUnit.KnowledgeType == knowledgeType.Value);

		var total = await query.CountAsync(cancellationToken);

		var items = await query
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.KnowledgeCollection)
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.ProficiencyLevel)
			.OrderBy(l => l.TranscriptSegment.Sequence)
			.ThenBy(l => l.StartTokenIndex)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}
}
