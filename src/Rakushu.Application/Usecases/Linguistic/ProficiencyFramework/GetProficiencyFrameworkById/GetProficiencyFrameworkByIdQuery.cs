using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.GetProficiencyFrameworkById;

public sealed record GetProficiencyFrameworkByIdQuery(Guid ProficiencyFrameworkId) : IRequest<Result<ProficiencyFrameworkDto>>;
