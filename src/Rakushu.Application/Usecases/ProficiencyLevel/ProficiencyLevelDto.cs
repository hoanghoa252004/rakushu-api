using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyLevel;

using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

namespace Rakushu.Application.Usecases.ProficiencyLevel;

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
