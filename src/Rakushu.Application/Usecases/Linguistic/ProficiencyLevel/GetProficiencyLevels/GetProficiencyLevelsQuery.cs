using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

public sealed record GetProficiencyLevelsQuery : IRequest<Result<IReadOnlyCollection<ProficiencyLevelDto>>>;
