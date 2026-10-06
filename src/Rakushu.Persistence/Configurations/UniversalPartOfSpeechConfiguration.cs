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
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(u => u.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName (from LinguisticMetadata base)
		builder.Property(u => u.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(u => u.Description);
	}
}
