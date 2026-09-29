using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Persistence.Configurations;

internal sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
	public void Configure(EntityTypeBuilder<MediaAsset> builder)
	{
		// Id
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(
				id => id.Value,
				value => MediaAssetId.From(value));

		// VideoId
		builder.Property(m => m.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne(m => m.Video)
			.WithMany(v => v.MediaAssets)
			.HasForeignKey(m => m.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		// AssetType
		builder.Property(m => m.AssetType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// StorageKey
		builder.Property(m => m.StorageKey)
			.HasMaxLength(500)
			.IsRequired();

		// ContentType
		builder.Property(m => m.ContentType)
			.HasMaxLength(100)
			.IsRequired();

		// FileName
		builder.Property(m => m.FileName)
			.HasMaxLength(255)
			.IsRequired();

		// FileSizeBytes
		builder.Property(m => m.FileSizeBytes)
			.IsRequired();

		// Container
		builder.Property(m => m.Container)
			.HasMaxLength(50);

		// Codec
		builder.Property(m => m.Codec)
			.HasMaxLength(50);

		// Width
		builder.Property(m => m.Width);

		// Height
		builder.Property(m => m.Height);

		// FrameRate
		builder.Property(m => m.FrameRate);

		// CreatedAt
		builder.Property(m => m.CreatedAt)
			.IsRequired();
	}
}
