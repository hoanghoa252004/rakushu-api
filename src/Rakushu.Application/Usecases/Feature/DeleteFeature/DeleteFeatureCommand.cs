using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Feature.DeleteFeature;

public sealed record DeleteFeatureCommand(Guid FeatureId) : IRequest<Result>;
