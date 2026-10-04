using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;

public record GetFeatureByIdQuery(Guid FeatureId) : IRequest<Result<FeatureDto>>;
