using Entity = Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset;

public sealed record MediaAssetDto(
	Guid VideoId,
	MediaAssetType AssetType,
	string StorageKey,
	string ContentType,
	string FileName,
	long FileSizeBytes)
{
	public static MediaAssetDto FromEntity(Entity e) =>
		new(
			e.VideoId.Value,
			e.AssetType,
			e.StorageKey,
			e.ContentType,
			e.FileName,
			e.FileSizeBytes);
}
