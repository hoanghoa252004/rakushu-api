using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Feature.CreateFeature;

public sealed record CreateFeatureCommand(
	string Code,
	string Name,
	string? Description = null
) : IRequest<Result<Guid>>;
