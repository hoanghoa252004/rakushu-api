using Entity = Rakushu.Domain.Entities.Video.MediaAsset.MediaAsset;

using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset;

public sealed record MediaAssetDto(
	Guid VideoId,
	MediaAssetType AssetType,
	string StorageKey,
	string ContentType,
	string FileName,
	long FileSizeBytes,
	string? Container,
	string? Codec,
	int? Width,
	int? Height,
	double? FrameRate)
{
	public static MediaAssetDto FromEntity(Entity e) =>
		new(
			e.VideoId.Value,
			e.AssetType,
			e.StorageKey,
			e.ContentType,
			e.FileName,
			e.FileSizeBytes,
			e.Container,
			e.Codec,
			e.Width,
			e.Height,
			e.FrameRate);
}
