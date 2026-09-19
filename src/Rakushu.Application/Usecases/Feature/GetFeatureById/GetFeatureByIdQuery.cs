using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Feature.GetFeatureById;

public record GetFeatureByIdQuery(Guid FeatureId) : IRequest<Result<FeatureDto>>;
