using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ProficiencyLevel;

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
			.HasMaxLength(30)
			.IsRequired();

		// Name
		builder.Property(pl => pl.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName
		builder.Property(pl => pl.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// SortOrder
		builder.Property(pl => pl.SortOrder)
			.IsRequired();

		// Description
		builder.Property(pl => pl.Description);

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
