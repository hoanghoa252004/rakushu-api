using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Feature.GetFeatures;

internal sealed class GetFeaturesHandler : IRequestHandler<GetFeaturesQuery, Result<PaginatedList<FeatureDto>>>
{
	// DAOs
	private readonly IFeatureQuery _featureQuery;

	public GetFeaturesHandler(
		IFeatureQuery featureQuery)
	{
		_featureQuery = featureQuery;
	}

	public async Task<Result<PaginatedList<FeatureDto>>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
	{
		// Parse status
		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<FeatureStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<FeatureDto>>(FeatureErrors.InvalidStatus);
		}

		var (items, totalCount) = await _featureQuery.GetFeaturesAsync(request, cancellationToken);

		var paginatedList = new PaginatedList<FeatureDto>(
			items.ToList(),
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
