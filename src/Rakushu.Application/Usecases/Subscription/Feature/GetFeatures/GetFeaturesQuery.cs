using MediatR;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatures;

public sealed record GetFeaturesQuery() : IRequest<Result<IReadOnlyCollection<FeatureDto>>>;
