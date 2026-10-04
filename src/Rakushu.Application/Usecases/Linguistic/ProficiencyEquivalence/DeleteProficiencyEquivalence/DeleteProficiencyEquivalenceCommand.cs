using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.DeleteProficiencyEquivalence;

public sealed record DeleteProficiencyEquivalenceCommand(Guid ProficiencyEquivalenceId) : IRequest<Result>;
