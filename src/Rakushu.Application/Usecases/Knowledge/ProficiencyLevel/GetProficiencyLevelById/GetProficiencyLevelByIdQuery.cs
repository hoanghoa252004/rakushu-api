using MediatR;
using Rakushu.Application.Usecases.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.ProficiencyLevel.GetProficiencyLevelById;

public sealed record GetProficiencyLevelByIdQuery(Guid ProficiencyLevelId) : IRequest<Result<ProficiencyLevelDto>>;
