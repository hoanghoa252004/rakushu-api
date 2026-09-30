using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.GetProficiencyFrameworks;

public sealed record GetProficiencyFrameworksQuery : IRequest<Result<IReadOnlyCollection<ProficiencyFrameworkDto>>>;
