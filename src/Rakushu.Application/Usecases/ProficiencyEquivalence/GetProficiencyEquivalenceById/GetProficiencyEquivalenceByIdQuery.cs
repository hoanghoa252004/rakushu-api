using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalenceById;

public sealed record GetProficiencyEquivalenceByIdQuery(Guid ProficiencyEquivalenceId) : IRequest<Result<ProficiencyEquivalenceDto>>;
