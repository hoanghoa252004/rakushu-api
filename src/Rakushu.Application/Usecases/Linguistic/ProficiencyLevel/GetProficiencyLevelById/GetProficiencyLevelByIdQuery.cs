using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

public sealed record GetProficiencyLevelByIdQuery(Guid ProficiencyLevelId) : IRequest<Result<ProficiencyLevelDto>>;
