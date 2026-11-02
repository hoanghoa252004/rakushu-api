using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingRoleConfiguration : IEntityTypeConfiguration<ShadowingRole>
{
	public void Configure(EntityTypeBuilder<ShadowingRole> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(r => r.Id);
		builder.Property(r => r.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleId.From(value));

		// ShadowingMaterialId
		builder.Property(r => r.ShadowingMaterialId)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialId.From(value))
			.IsRequired();
		builder.HasOne(r => r.ShadowingMaterial)
			.WithMany(m => m.Roles)
			.HasForeignKey(r => r.ShadowingMaterialId)
			.OnDelete(DeleteBehavior.Cascade);

		// SpeakerId
		builder.Property(r => r.SpeakerId)
			.HasConversion(
				id => id.Value,
				value => SpeakerId.From(value))
			.IsRequired();
		builder.HasOne(r => r.Speaker)
			.WithMany()
			.HasForeignKey(r => r.SpeakerId)
			.OnDelete(DeleteBehavior.Restrict);

		// SortOrder
		builder.Property(r => r.SortOrder)
			.IsRequired();

		// Unique: one speaker per material, unique order per material
		builder.HasIndex(r => new { r.ShadowingMaterialId, r.SpeakerId })
			.IsUnique();
		builder.HasIndex(r => new { r.ShadowingMaterialId, r.SortOrder })
			.IsUnique();
	}
}
