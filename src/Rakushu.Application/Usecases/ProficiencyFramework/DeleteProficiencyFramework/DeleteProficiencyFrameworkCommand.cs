using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.DeleteProficiencyFramework;

public sealed record DeleteProficiencyFrameworkCommand(Guid ProficiencyFrameworkId) : IRequest<Result>;
