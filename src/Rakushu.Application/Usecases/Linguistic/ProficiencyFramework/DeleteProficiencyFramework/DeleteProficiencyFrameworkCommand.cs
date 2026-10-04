using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.DeleteProficiencyFramework;

public sealed record DeleteProficiencyFrameworkCommand(Guid ProficiencyFrameworkId) : IRequest<Result>;
