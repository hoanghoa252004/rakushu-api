using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series;

public sealed record SeriesDto(
	Guid Id,
	Guid? UserId,
	string Slug,
	string Name,
	string JapaneseName,
	string? ThumbnailKey,
	string? Description,
	int DisplayOrder,
	string SourceType,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt)
{
	public static SeriesDto FromEntity(Domain.Entities.Material.Series.Series e) =>
		new(
			e.Id.Value,
			e.User?.Id.Value,
			e.Slug,
			e.Name,
			e.JapaneseName,
			e.ThumbnailKey,
			e.Description,
			e.DisplayOrder,
			e.SourceType.ToString(),
			e.IsActive,
			e.CreatedAt,
			e.UpdatedAt);
}
