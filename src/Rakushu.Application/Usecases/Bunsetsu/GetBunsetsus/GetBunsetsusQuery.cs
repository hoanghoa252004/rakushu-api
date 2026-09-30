using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Bunsetsu.GetBunsetsus;

public sealed record GetBunsetsusQuery : IRequest<Result<IReadOnlyCollection<BunsetsuDto>>>;
