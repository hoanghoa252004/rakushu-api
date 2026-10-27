namespace Rakushu.Application.Usecases.Knowledge.ProficiencyLevel;

public sealed record ProficiencyLevelDto(
	Guid Id,
	string Code,
	string Name,
	string JapaneseName,
	int SortOrder,
	bool IsActive,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
	)
{
	internal static ProficiencyLevelDto From(Domain.Entities.Knowledge.ProficiencyLevel.ProficiencyLevel level) =>
		new(
			level.Id.Value,
			level.Code,
			level.Name,
			level.JapaneseName,
			level.SortOrder,
			level.IsActive,
			level.Description,
			level.CreatedAt,
			level.UpdatedAt);
}
