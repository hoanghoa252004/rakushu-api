using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProficiencyLevel.GetProficiencyLevelById;

public sealed record GetProficiencyLevelByIdQuery(Guid ProficiencyLevelId) : IRequest<Result<ProficiencyLevelDto>>;
