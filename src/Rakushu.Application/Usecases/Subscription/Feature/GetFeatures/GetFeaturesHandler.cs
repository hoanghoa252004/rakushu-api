using MediatR;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatures;

internal sealed class GetFeaturesHandler : IRequestHandler<GetFeaturesQuery, Result<IReadOnlyCollection<FeatureDto>>>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;

	public GetFeaturesHandler(
		IFeatureRepository featureRepository)
	{
		_featureRepository = featureRepository;
	}

	public async Task<Result<IReadOnlyCollection<FeatureDto>>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
	{
		var features = await _featureRepository.GetAllAsync();

		var featureDtos = features.Select(f => new FeatureDto(
			f.Id.Value,
			f.Code,
			f.Name,
			f.IsActive,
			f.CreatedAt,
			f.UpdatedAt,
			f.Description
		)).ToList();

		return Result.Success<IReadOnlyCollection<FeatureDto>>(featureDtos);
	}
}
