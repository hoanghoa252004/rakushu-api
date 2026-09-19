using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Feature.ObjectValues;
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
		builder.Property(u => u.Code)
			.HasConversion(
				code => code.Value,
				value => FeatureCode.Create(value).Value)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name
		builder.Property(u => u.Name)
			.IsRequired();

		// Description
		builder.Property(u => u.Description);

		// Status
		builder.Property(u => u.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}
