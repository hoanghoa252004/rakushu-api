using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Knowledge.DependencyRelationship;

public interface IDependencyRelationshipRepository : IBaseRepository<DependencyRelationship, DependencyRelationshipId>
{
	Task<DependencyRelationship?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
