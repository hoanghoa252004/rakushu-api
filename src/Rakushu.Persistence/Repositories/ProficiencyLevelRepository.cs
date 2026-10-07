using Rakushu.Domain.Entities.ProficiencyLevel;

namespace Rakushu.Persistence.Repositories;

public sealed class ProficiencyLevelRepository : BaseRepository<ProficiencyLevel, ProficiencyLevelId>, IProficiencyLevelRepository
{
	public ProficiencyLevelRepository(RakushuDbContext context) : base(context) { }
}
