using Rakushu.Domain.Entities.Series;

namespace Rakushu.Application.Usecases.Learning.Series;

public sealed record SeriesDto(
	Guid Id,
	string Slug,
	string Name,
	string? Description,
	int DisplayOrder,
	SeriesSource SourceType,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt)
{
	public static SeriesDto FromEntity(Domain.Entities.Series.Series e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Name,
			e.Description,
			e.DisplayOrder,
			e.SourceType,
			e.IsActive,
			e.CreatedAt,
			e.UpdatedAt);
}
