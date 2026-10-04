using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

public sealed record GetProficiencyLevelByIdQuery(Guid ProficiencyLevelId) : IRequest<Result<ProficiencyLevelDto>>;
