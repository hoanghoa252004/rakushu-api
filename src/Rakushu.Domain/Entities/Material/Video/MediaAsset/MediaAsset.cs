using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Material.Video.MediaAsset;

public sealed partial class MediaAsset : Entity<MediaAssetId>
{
	public VideoId VideoId { get; private set; } = null!;
	public MediaAssetType AssetType { get; private set; }
	public string StorageKey { get; private set; } = null!;
	public string ContentType { get; private set; } = null!;
	public string FileName { get; private set; } = null!;
	public long FileSizeBytes { get; private set; }

	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private MediaAsset() { }

	private MediaAsset(
		MediaAssetId id,
		VideoId videoId,
		MediaAssetType assetType,
		string storageKey,
		string contentType,
		string fileName,
		long fileSizeBytes) : base(id)
	{
		VideoId = videoId;
		AssetType = assetType;
		StorageKey = storageKey;
		ContentType = contentType;
		FileName = fileName;
		FileSizeBytes = fileSizeBytes;
	}

	public static Result<MediaAsset> Create(
		VideoId videoId,
		MediaAssetType assetType,
		string storageKey,
		string contentType,
		string fileName,
		long fileSizeBytes)
	{
		return Result.Success(new MediaAsset(
			MediaAssetId.Create(),
			videoId,
			assetType,
			storageKey,
			contentType,
			fileName,
			fileSizeBytes));
	}

	public Result Update(
		MediaAssetType assetType,
		string storageKey,
		string contentType,
		string fileName,
		long fileSizeBytes
		)
	{
		AssetType = assetType;
		StorageKey = storageKey;
		ContentType = contentType;
		FileName = fileName;
		FileSizeBytes = fileSizeBytes;
		return Result.Success();
	}
}