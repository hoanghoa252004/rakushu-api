using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;

public sealed record ProficiencyLevelDto(
	string Code,
	string Name,
	int SortOrder,
	string? Description)
{
	public static ProficiencyLevelDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.SortOrder,
			e.Description);
}
