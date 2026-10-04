using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;

public interface IProficiencyFrameworkRepository : IBaseRepository<ProficiencyFramework, ProficiencyFrameworkId>
{
	Task<ProficiencyFramework?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
	Task<ProficiencyFramework?> GetByLevelIdAsync(ProficiencyLevelId levelId, CancellationToken cancellationToken = default);
}
