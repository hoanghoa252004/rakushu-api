using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class TranscriptConfiguration : IEntityTypeConfiguration<Transcript>
{
	public void Configure(EntityTypeBuilder<Transcript> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => TranscriptId.From(value));

		// VideoId
		builder.Property(t => t.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne(t => t.Video)
			.WithOne(v => v.Transcript)
			.HasForeignKey<Transcript>(t => t.VideoId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasIndex(t => t.VideoId)
			.IsUnique();

		// FullText
		builder.Property(t => t.FullText)
			.IsRequired();

		// SourceType
		builder.Property(v => v.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// StorageKey
		builder.Property(m => m.StorageKey)
			.HasMaxLength(100);

		// IsReviewed
		builder.Property(t => t.IsReviewed)
			.HasDefaultValue(false)
			.IsRequired();

		// CreatedAt
		builder.Property(t => t.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(t => t.UpdatedAt)
			.IsRequired();

	}
}
