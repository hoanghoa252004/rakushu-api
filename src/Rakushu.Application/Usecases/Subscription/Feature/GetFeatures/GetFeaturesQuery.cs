using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatures;

public sealed record GetFeaturesQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchTerm = null,
	string? Status = null
) : IRequest<Result<PaginatedList<FeatureDto>>>;
