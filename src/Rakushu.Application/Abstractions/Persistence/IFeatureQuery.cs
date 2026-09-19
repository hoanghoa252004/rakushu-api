using Rakushu.Application.Usecases.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Feature.GetFeatures;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IFeatureQuery
{
	Task<FeatureDto?> GetByIdAsync(FeatureId id, CancellationToken cancellationToken = default);
	Task<(IReadOnlyCollection<FeatureDto> Items, int TotalCount)> GetFeaturesAsync(GetFeaturesQuery query, CancellationToken cancellationToken = default);
}
