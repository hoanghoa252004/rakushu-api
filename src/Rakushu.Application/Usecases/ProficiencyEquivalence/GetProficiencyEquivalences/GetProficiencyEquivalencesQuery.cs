using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalences;

public sealed record GetProficiencyEquivalencesQuery : IRequest<Result<IReadOnlyCollection<ProficiencyEquivalenceDto>>>;
