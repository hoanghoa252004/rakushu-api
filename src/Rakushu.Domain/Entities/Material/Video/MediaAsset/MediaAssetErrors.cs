using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Material.Video.MediaAsset;

public static class MediaAssetErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"MEDIA_ASSET.NOT_FOUND", "The media asset was not found.");
}
