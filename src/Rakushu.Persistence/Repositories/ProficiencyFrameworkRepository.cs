using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Persistence.Repositories;

public sealed class ProficiencyFrameworkRepository : BaseRepository<ProficiencyFramework, ProficiencyFrameworkId>, IProficiencyFrameworkRepository
{
	public ProficiencyFrameworkRepository(RakushuDbContext context) : base(context) { }

	public override async Task<ProficiencyFramework?> GetByIdAsync(ProficiencyFrameworkId id, CancellationToken cancellationToken = default)
	{
		return await _context.ProficiencyFrameworks
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.SourceLevelProficiencyEquivalences)
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.TargetLevelProficiencyEquivalences)
			.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
	}

	public async Task<ProficiencyFramework?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.ProficiencyFrameworks
			.Include(f => f.ProficiencyLevels)
			.FirstOrDefaultAsync(f => f.Code == code, cancellationToken);
	}

	public async Task<ProficiencyFramework?> GetByLevelIdAsync(ProficiencyLevelId levelId, CancellationToken cancellationToken = default)
	{
		return await _context.ProficiencyFrameworks
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.SourceLevelProficiencyEquivalences)
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.TargetLevelProficiencyEquivalences)
			.FirstOrDefaultAsync(f => f.ProficiencyLevels.Any(l => l.Id == levelId), cancellationToken);
	}

	public override async Task<IEnumerable<ProficiencyFramework>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.ProficiencyFrameworks
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.SourceLevelProficiencyEquivalences)
			.Include(f => f.ProficiencyLevels)
				.ThenInclude(l => l.TargetLevelProficiencyEquivalences)
			.ToListAsync(cancellationToken);
	}
}
