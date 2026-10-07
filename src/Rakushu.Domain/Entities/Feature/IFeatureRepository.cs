using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Feature;

public interface IFeatureRepository : IBaseRepository<Feature, FeatureId>
{
	Task<Feature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
