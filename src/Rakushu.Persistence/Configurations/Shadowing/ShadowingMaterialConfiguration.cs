using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingMaterialConfiguration : IEntityTypeConfiguration<ShadowingMaterial>
{
	public void Configure(EntityTypeBuilder<ShadowingMaterial> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialId.From(value));

		// Code
		builder.Property(m => m.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(m => m.Code)
			.IsUnique();

		// VideoId
		builder.Property(m => m.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne<Video>()
			.WithMany()
			.HasForeignKey(m => m.VideoId)
			.OnDelete(DeleteBehavior.Restrict);

		// Version
		builder.Property(m => m.Version)
			.IsRequired();
		builder.HasIndex(m => new { m.VideoId, m.Version })
			.IsUnique();

		// IsActive
		builder.Property(m => m.IsActive)
			.HasDefaultValue(false)
			.IsRequired();

		// CreatedAt
		builder.Property(m => m.CreatedAt)
			.IsRequired();
	}
}
