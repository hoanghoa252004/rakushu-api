using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Feature.CreateFeature;

public sealed record CreateFeatureCommand(
	string Code,
	string Name,
	bool IsActive,
	string? Description = null
) : IRequest<Result<Guid>>;
