using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

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

		// Type (enum)
		builder.Property(s => s.Type)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Status (enum)
		builder.Property(s => s.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// VideoId
		builder.Property(m => m.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne<Video>(x => x.Video)
			.WithMany(v => v.ShadowingMaterials)
			.HasForeignKey(m => m.VideoId)
			.OnDelete(DeleteBehavior.Restrict);

		// ShadowingMaterialRequestId
		builder.Property(m => m.ShadowingMaterialRequestId)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialRequestId.From(value))
			.IsRequired();
		builder.HasOne(x => x.ShadowingMaterialRequest)
			.WithMany(v => v.ShadowingMaterials)
			.HasForeignKey(m => m.ShadowingMaterialRequestId)
			.OnDelete(DeleteBehavior.Restrict);

		// Version
		builder.Property(m => m.Version)
			.IsRequired();
		builder.HasIndex(m => new { m.VideoId, m.Version })
			.IsUnique();

		// CreatedAt
		builder.Property(m => m.CreatedAt)
			.IsRequired();
	}
}
