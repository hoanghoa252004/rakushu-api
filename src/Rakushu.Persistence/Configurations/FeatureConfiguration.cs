using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
	public void Configure(EntityTypeBuilder<Feature> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => FeatureId.From(value));

		// Code
		builder.Property(pl => pl.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name
		builder.Property(u => u.Name)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(u => u.Description);

		// IsActive
		builder.Property(pl => pl.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}
