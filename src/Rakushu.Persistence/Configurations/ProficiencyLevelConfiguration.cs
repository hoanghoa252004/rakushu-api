using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Persistence.Configurations;

internal sealed class ProficiencyLevelConfiguration : IEntityTypeConfiguration<ProficiencyLevel>
{
	public void Configure(EntityTypeBuilder<ProficiencyLevel> builder)
	{
		// Id
		builder.HasKey(pl => pl.Id);
		builder.Property(pl => pl.Id)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value));

		// Code
		builder.Property(pl => pl.Code)
			.HasMaxLength(50)
			.IsRequired();

		// Name
		builder.Property(pl => pl.Name)
			.HasMaxLength(200)
			.IsRequired();

		// SortOrder
		builder.Property(pl => pl.SortOrder)
			.IsRequired();

		// Description
		builder.Property(pl => pl.Description);

		// ProficiencyFrameworkId
		builder.Property(i => i.ProficiencyFrameworkId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyFrameworkId.From(value))
			.IsRequired();
		builder.HasOne(i => i.Framework)
			.WithMany(p => p.ProficiencyLevels)
			.HasForeignKey(i => i.ProficiencyFrameworkId)
			.OnDelete(DeleteBehavior.Cascade);

		// IsActive
		builder.Property(pl => pl.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(pl => pl.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(pl => pl.UpdatedAt)
			.IsRequired();
	}
}
