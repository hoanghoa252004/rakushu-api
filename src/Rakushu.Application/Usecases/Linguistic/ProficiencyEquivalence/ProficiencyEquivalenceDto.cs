using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence.ProficiencyEquivalence;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence;

public sealed record ProficiencyEquivalenceDto(
	Guid SourceLevelId,
	Guid TargetLevelId,
	EquivalenceType Type,
	string? Note,
	string? Reference)
{
	public static ProficiencyEquivalenceDto FromEntity(Entity e) =>
		new(
			e.SourceLevelId.Value,
			e.TargetLevelId.Value,
			e.Type,
			e.Note,
			e.Reference);
}
