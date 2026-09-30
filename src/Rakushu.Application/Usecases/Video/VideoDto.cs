using Entity = Rakushu.Domain.Entities.Video.Video;

using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Video;

public sealed record VideoDto(
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid SeriesId,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	VideoStatus Status,
	Guid CreatedBy)
{
	public static VideoDto FromEntity(Entity e) =>
		new(
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			e.ContentCategoryId.Value,
			e.SeriesId.Value,
			e.SortOrder,
			e.SourceType,
			e.SourceUrl,
			e.Status,
			e.CreatedBy.Value);
}
