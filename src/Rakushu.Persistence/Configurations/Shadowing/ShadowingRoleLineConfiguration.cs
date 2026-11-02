using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingRoleLineConfiguration : IEntityTypeConfiguration<ShadowingRoleLine>
{
	public void Configure(EntityTypeBuilder<ShadowingRoleLine> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(l => l.Id);
		builder.Property(l => l.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleLineId.From(value));

		// ShadowingMaterialId
		builder.Property(l => l.ShadowingMaterialId)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialId.From(value))
			.IsRequired();
		builder.HasOne(l => l.ShadowingMaterial)
			.WithMany(m => m.RoleLines)
			.HasForeignKey(l => l.ShadowingMaterialId)
			.OnDelete(DeleteBehavior.Restrict);

		// ShadowingRoleId
		builder.Property(l => l.ShadowingRoleId)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleId.From(value))
			.IsRequired();
		builder.HasOne(l => l.ShadowingRole)
			.WithMany(r => r.RoleLines)
			.HasForeignKey(l => l.ShadowingRoleId)
			.OnDelete(DeleteBehavior.Cascade);

		// SortOrder
		builder.Property(l => l.SortOrder)
			.IsRequired();
		builder.HasIndex(l => new { l.ShadowingMaterialId, l.SortOrder })
			.IsUnique();

		// Text
		builder.Property(l => l.Text)
			.IsRequired();

		// StartTime
		builder.Property(l => l.StartTime)
			.IsRequired();

		// EndTime
		builder.Property(l => l.EndTime)
			.IsRequired();

		// AudioKey
		builder.Property(l => l.AudioKey)
			.HasMaxLength(500)
			.IsRequired();

		// Duration (computed)
		builder.Ignore(l => l.Duration);
	}
}
