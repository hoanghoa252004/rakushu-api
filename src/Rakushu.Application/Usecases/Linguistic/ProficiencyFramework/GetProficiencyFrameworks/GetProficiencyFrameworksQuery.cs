using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.GetProficiencyFrameworks;

public sealed record GetProficiencyFrameworksQuery : IRequest<Result<IReadOnlyCollection<ProficiencyFrameworkDto>>>;
