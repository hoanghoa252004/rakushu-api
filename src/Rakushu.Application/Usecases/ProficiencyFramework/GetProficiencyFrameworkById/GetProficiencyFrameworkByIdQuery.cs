using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.GetProficiencyFrameworkById;

public sealed record GetProficiencyFrameworkByIdQuery(Guid ProficiencyFrameworkId) : IRequest<Result<ProficiencyFrameworkDto>>;
