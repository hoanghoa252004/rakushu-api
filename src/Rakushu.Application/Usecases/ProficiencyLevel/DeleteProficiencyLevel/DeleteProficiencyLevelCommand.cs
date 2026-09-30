using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

namespace Rakushu.Application.Usecases.ProficiencyLevel.DeleteProficiencyLevel;

public sealed record DeleteProficiencyLevelCommand(Guid ProficiencyLevelId) : IRequest<Result>;
