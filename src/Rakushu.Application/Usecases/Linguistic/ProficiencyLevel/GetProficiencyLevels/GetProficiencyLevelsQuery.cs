using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

public sealed record GetProficiencyLevelsQuery : IRequest<Result<IReadOnlyCollection<ProficiencyLevelDto>>>;
