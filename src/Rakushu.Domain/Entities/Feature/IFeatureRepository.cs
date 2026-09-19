using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Feature.ObjectValues;

namespace Rakushu.Domain.Entities.Feature;

public interface IFeatureRepository : IBaseRepository<Feature, FeatureId>
{
	Task<Feature?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken = default);
}
