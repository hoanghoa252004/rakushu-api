using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

namespace Rakushu.Application.Usecases.Learning.Bunsetsu.DeleteBunsetsu;

public sealed record DeleteBunsetsuCommand(Guid BunsetsuId) : IRequest<Result>;
