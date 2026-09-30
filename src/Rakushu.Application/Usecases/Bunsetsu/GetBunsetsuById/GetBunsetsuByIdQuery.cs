using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Bunsetsu.GetBunsetsuById;

public sealed record GetBunsetsuByIdQuery(Guid BunsetsuId) : IRequest<Result<BunsetsuDto>>;
