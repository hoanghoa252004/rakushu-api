using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Feature.GetFeatureById;

namespace Rakushu.Application.Usecases.Feature.GetFeatures;

public sealed record GetFeaturesQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchTerm = null,
	string? Status = null
) : IRequest<Result<PaginatedList<FeatureDto>>>;

