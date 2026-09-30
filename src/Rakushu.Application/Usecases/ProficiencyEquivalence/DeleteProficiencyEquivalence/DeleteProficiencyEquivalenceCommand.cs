using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.DeleteProficiencyEquivalence;

public sealed record DeleteProficiencyEquivalenceCommand(Guid ProficiencyEquivalenceId) : IRequest<Result>;
