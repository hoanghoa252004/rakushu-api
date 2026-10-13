using Entity = Rakushu.Domain.Entities.Material.Video.Video;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Video;

public sealed record VideoDto(
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	VideoStatus Status,
	Guid? LearnerId)
{
	public static VideoDto FromEntity(Entity e) =>
		new(
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			e.ContentCategoryId.Value,
			e.SeriesId?.Value,
			e.SortOrder,
			e.SourceType,
			e.SourceUrl,
			e.Status,
			e.LearnerId?.Value);
}
