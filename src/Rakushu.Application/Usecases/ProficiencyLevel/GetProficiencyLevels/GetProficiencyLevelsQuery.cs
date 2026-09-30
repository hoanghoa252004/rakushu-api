using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProficiencyLevel.GetProficiencyLevels;

public sealed record GetProficiencyLevelsQuery : IRequest<Result<IReadOnlyCollection<ProficiencyLevelDto>>>;
