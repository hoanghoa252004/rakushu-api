using MediatR;
using Rakushu.Application.Usecases.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.ProficiencyLevel.GetProficiencyLevels;

public sealed record GetProficiencyLevelsQuery : IRequest<Result<IReadOnlyCollection<ProficiencyLevelDto>>>;
