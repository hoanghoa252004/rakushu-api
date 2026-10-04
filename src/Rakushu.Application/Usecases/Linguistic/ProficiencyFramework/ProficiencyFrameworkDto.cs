using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyFramework.ProficiencyFramework;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework;

public sealed record ProficiencyFrameworkDto(
	string Code,
	string Name,
	string? Description,
	bool IsActive)
{
	public static ProficiencyFrameworkDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.Description,
			e.IsActive);
}
