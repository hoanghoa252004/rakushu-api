using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

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
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(dr => dr.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(dr => dr.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName (from LinguisticMetadata base)
		builder.Property(dr => dr.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(dr => dr.Description);
	}
}
