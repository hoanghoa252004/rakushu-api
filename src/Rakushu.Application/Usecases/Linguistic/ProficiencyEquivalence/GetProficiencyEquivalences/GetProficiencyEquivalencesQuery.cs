using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.GetProficiencyEquivalences;

public sealed record GetProficiencyEquivalencesQuery : IRequest<Result<IReadOnlyCollection<ProficiencyEquivalenceDto>>>;
