using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.Bunsetsu;

namespace Rakushu.Application.Usecases.Learning.Bunsetsu.GetBunsetsuById;

public sealed record GetBunsetsuByIdQuery(Guid BunsetsuId) : IRequest<Result<BunsetsuDto>>;
