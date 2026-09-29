using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Persistence.Configurations;

internal sealed class UniversalPartOfSpeechConfiguration : IEntityTypeConfiguration<UniversalPartOfSpeech>
{
	public void Configure(EntityTypeBuilder<UniversalPartOfSpeech> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => UniversalPartOfSpeechId.From(value));

		// Code (from LinguisticMetadata base)
		builder.Property(u => u.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(u => u.Name)
			.HasMaxLength(200)
			.IsRequired();

		// VietnameseName (from LinguisticMetadata base)
		builder.Property(u => u.VietnameseName)
			.HasMaxLength(200)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(u => u.Description)
			.HasMaxLength(1000);
	}
}
