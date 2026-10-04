using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Persistence.Configurations;

internal sealed class DependencyRelationshipConfiguration : IEntityTypeConfiguration<DependencyRelationship>
{
	public void Configure(EntityTypeBuilder<DependencyRelationship> builder)
	{
		// Id
		builder.HasKey(dr => dr.Id);
		builder.Property(dr => dr.Id)
			.HasConversion(
				id => id.Value,
				value => DependencyRelationshipId.From(value));

		// Code (from LinguisticMetadata base)
		builder.Property(dr => dr.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(dr => dr.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(dr => dr.Name)
			.HasMaxLength(200)
			.IsRequired();

		// VietnameseName (from LinguisticMetadata base)
		builder.Property(dr => dr.VietnameseName)
			.HasMaxLength(200)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(dr => dr.Description)
			.HasMaxLength(1000);
	}
}
