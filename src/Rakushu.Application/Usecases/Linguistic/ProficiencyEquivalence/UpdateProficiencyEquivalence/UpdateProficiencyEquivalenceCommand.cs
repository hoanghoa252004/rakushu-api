using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.UpdateProficiencyEquivalence;

public sealed record UpdateProficiencyEquivalenceCommand(
	Guid ProficiencyEquivalenceId,
	Guid sourceLevelId,
	Guid targetLevelId,
	EquivalenceType type,
	string? note,
	string? reference
) : IRequest<Result>;
