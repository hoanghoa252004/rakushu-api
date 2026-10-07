namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;

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
	);
