using Entity = Rakushu.Domain.Entities.Material.Video.Video;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Application.Usecases.Learning.Series;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;

namespace Rakushu.Application.Usecases.Learning.Video;

public sealed record VideoDto(
	Guid Id,
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	ContentCategoryDto ContentCategory,
	SeriesDto? Series,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	VideoStatus Status,
	UserBasicDto? Learner)
{
	public static VideoDto FromEntity(Entity e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			new ContentCategoryDto(
				e.ContentCategory.Id.Value,
				e.ContentCategory.Slug,
				e.ContentCategory.Code,
				e.ContentCategory.Name,
				e.ContentCategory.JapaneseName,
				e.ContentCategory.Description,
				e.ContentCategory.DisplayOrder,
				e.ContentCategory.IsActive,
				e.ContentCategory.CreatedAt,
				e.ContentCategory.UpdatedAt
				),
			e.SeriesId != null ? SeriesDto.FromEntity(e.Series!) : null,
			e.SortOrder,
			e.SourceType,
			e.SourceUrl,
			e.Status,
			e.Learner != null ? new UserBasicDto(
				e.Learner.Id.Value,
				e.Learner.FullName,
				e.Learner.Email
				) : null
			);
}

public sealed record VideoDetailDto(
	Guid Id,
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	ContentCategoryDto ContentCategory,
	SeriesDto? Series,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	VideoStatus Status,
	UserBasicDto? Learner,
	IReadOnlyCollection<MediaAssetDto> MediaAssets)
{
	public static VideoDetailDto FromEntity(Entity e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Title,
			e.Description,
			e.Duration,
			new ContentCategoryDto(
				e.ContentCategory.Id.Value,
				e.ContentCategory.Slug,
				e.ContentCategory.Code,
				e.ContentCategory.Name,
				e.ContentCategory.JapaneseName,
				e.ContentCategory.Description,
				e.ContentCategory.DisplayOrder,
				e.ContentCategory.IsActive,
				e.ContentCategory.CreatedAt,
				e.ContentCategory.UpdatedAt
				),
			e.SeriesId != null ? SeriesDto.FromEntity(e.Series!) : null,
			e.SortOrder,
			e.SourceType,
			e.SourceUrl,
			e.Status,
			e.Learner != null ? new UserBasicDto(
				e.Learner.Id.Value,
				e.Learner.FullName,
				e.Learner.Email
				) : null,
			e.MediaAssets.Select(ma => new MediaAssetDto(
				ma.Id.Value,
				ma.StorageKey,
				ma.ContentType,
				ma.AssetType.ToString()
				)).ToList()
			);
}

public sealed record MediaAssetDto(
	Guid Id,
	string StorageKey,
	string ContentType,
	string MediaType);
