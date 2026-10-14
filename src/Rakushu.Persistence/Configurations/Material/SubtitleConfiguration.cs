using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
{
	public void Configure(EntityTypeBuilder<Subtitle> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => SubtitleId.From(value));

		// Code
		builder.Property(s => s.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(s => s.Code)
			.IsUnique();

		// VideoId
		builder.Property(s => s.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne(s => s.Video)
			.WithMany(v => v.Subtitles)
			.HasForeignKey(s => s.VideoId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasIndex(t => t.VideoId)
			.IsUnique();

		// CreatedAt
		builder.Property(s => s.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(s => s.UpdatedAt)
			.IsRequired();
	}
}
