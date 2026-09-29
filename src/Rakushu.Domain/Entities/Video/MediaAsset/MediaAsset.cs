using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.MediaAsset;

public sealed class MediaAsset : Entity<MediaAssetId>
{
	public VideoId VideoId { get; private set; } = null!;
	public MediaAssetType AssetType { get; private set; }
	public string StorageKey { get; private set; } = null!;
	public string ContentType { get; private set; } = null!;
	public string FileName { get; private set; } = null!;
	public long FileSizeBytes { get; private set; }
	public string? Container { get; private set; }
	public string? Codec { get; private set; }
	public int? Width { get; private set; }
	public int? Height { get; private set; }
	public double? FrameRate { get; private set; }
	public DateTime CreatedAt { get; private set; }

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
		long fileSizeBytes,
		DateTime createdAt,
		string? container = null,
		string? codec = null,
		int? width = null,
		int? height = null,
		double? frameRate = null) : base(id)
	{
		VideoId = videoId;
		AssetType = assetType;
		StorageKey = storageKey;
		ContentType = contentType;
		FileName = fileName;
		FileSizeBytes = fileSizeBytes;
		Container = container;
		Codec = codec;
		Width = width;
		Height = height;
		FrameRate = frameRate;
		CreatedAt = createdAt;
	}
}
