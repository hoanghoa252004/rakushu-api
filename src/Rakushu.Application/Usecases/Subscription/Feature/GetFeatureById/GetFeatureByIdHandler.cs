using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;

internal sealed class GetFeatureByIdHandler : IRequestHandler<GetFeatureByIdQuery, Result<FeatureDto>>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;

	public GetFeatureByIdHandler(IFeatureRepository featureRepository)
	{
		_featureRepository = featureRepository;
	}

	public async Task<Result<FeatureDto>> Handle(GetFeatureByIdQuery request, CancellationToken cancellationToken)
	{
		var id = FeatureId.From(request.FeatureId);

		var f = await _featureRepository.GetByIdAsync(id, cancellationToken);

		if (f == null)
		{
			return Result.Failure<FeatureDto>(FeatureErrors.NotFound);
		}

		return Result.Success(new FeatureDto(
			f.Id.Value,
			f.Code,
			f.Name,
			f.IsActive,
			f.CreatedAt,
			f.UpdatedAt,
			f.Description));
	}
}
