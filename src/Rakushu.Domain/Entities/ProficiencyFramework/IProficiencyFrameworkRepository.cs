using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

namespace Rakushu.Domain.Entities.ProficiencyFramework;

public interface IProficiencyFrameworkRepository : IBaseRepository<ProficiencyFramework, ProficiencyFrameworkId>
{
	Task<ProficiencyFramework?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
	Task<ProficiencyFramework?> GetByLevelIdAsync(ProficiencyLevelId levelId, CancellationToken cancellationToken = default);
}
