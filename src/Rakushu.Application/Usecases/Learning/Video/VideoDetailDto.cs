using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video;

public sealed record VideoListDto(
	Guid Id,
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder,
	VideoSource SourceType,
	VideoStatus Status,
	Guid CreatedBy,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt)
{
	public static VideoListDto FromEntity(Domain.Entities.Video.Video e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			e.ContentCategoryId.Value,
			e.SeriesId?.Value,
			e.SortOrder,
			e.SourceType,
			e.Status,
			e.CreatedBy.Value,
			e.CreatedAt,
			e.UpdatedAt);
}

public sealed record VideoDetailDto(
	Guid Id,
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	string? ContentCategoryName,
	Guid? SeriesId,
	string? SeriesName,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	VideoStatus Status,
	Guid CreatedBy,
	string? CreatedByFullName,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	List<MediaAssetDto> MediaAssets)
{
	public static VideoDetailDto FromEntity(Domain.Entities.Video.Video e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			e.ContentCategoryId.Value,
			e.ContentCategory?.Name,
			e.SeriesId?.Value,
			e.Series?.Name,
			e.SortOrder,
			e.SourceType,
			e.SourceUrl,
			e.Status,
			e.CreatedBy.Value,
			e.CreatedByUser?.FullName,
			e.CreatedAt,
			e.UpdatedAt,
			e.MediaAssets.Select(ma => new MediaAssetDto(
				ma.Id.Value,
				ma.StorageKey,
				ma.ContentType,
				ma.AssetType.ToString())).ToList());
}

public sealed record MediaAssetDto(
	Guid Id,
	string StorageKey,
	string ContentType,
	string MediaType);

