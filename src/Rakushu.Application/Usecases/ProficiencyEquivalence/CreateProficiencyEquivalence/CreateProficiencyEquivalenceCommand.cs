using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.CreateProficiencyEquivalence;

public sealed record CreateProficiencyEquivalenceCommand(
	Guid sourceLevelId,
	Guid targetLevelId,
	EquivalenceType type,
	string? note,
	string? reference
) : IRequest<Result<Guid>>;
