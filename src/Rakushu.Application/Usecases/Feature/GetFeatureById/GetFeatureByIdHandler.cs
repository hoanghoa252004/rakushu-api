using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Feature.GetFeatureById;

internal sealed class GetFeatureByIdHandler : IRequestHandler<GetFeatureByIdQuery, Result<FeatureDto>>
{
	// DAOs
	private readonly IFeatureQuery _featureQuery;

	public GetFeatureByIdHandler(IFeatureQuery featureQuery)
	{
		_featureQuery = featureQuery;
	}

	public async Task<Result<FeatureDto>> Handle(GetFeatureByIdQuery request, CancellationToken cancellationToken)
	{
		var feature = await _featureQuery.GetByIdAsync(
			FeatureId.From(request.FeatureId),
			cancellationToken);

		if (feature == null)
		{
			return Result.Failure<FeatureDto>(FeatureErrors.NotFound);
		}

		return Result.Success(feature);
	}
}
