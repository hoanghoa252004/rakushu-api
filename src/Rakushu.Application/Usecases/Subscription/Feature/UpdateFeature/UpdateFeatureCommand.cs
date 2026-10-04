using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Feature.UpdateFeature;

public sealed record UpdateFeatureCommand(
	Guid FeatureId,
	string Name,
	string? Description = null
) : IRequest<Result>;
