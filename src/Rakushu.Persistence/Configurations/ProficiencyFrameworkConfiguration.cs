using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Persistence.Configurations;

internal sealed class ProficiencyFrameworkConfiguration : IEntityTypeConfiguration<ProficiencyFramework>
{
	public void Configure(EntityTypeBuilder<ProficiencyFramework> builder)
	{
		// Id
		builder.HasKey(pf => pf.Id);
		builder.Property(pf => pf.Id)
			.HasConversion(
				id => id.Value,
				value => ProficiencyFrameworkId.From(value));

		// Code
		builder.Property(pf => pf.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(pf => pf.Code)
			.IsUnique();

		// Name
		builder.Property(pf => pf.Name)
			.HasMaxLength(200)
			.IsRequired();

		// Description
		builder.Property(pf => pf.Description)
			.HasMaxLength(1000);

		// IsActive
		builder.Property(pf => pf.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(pf => pf.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(pf => pf.UpdatedAt)
			.IsRequired();
	}
}
