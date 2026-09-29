using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Persistence.Configurations;

internal sealed class JapanesePartOfSpeechConfiguration : IEntityTypeConfiguration<JapanesePartOfSpeech>
{
	public void Configure(EntityTypeBuilder<JapanesePartOfSpeech> builder)
	{
		// Id
		builder.HasKey(j => j.Id);
		builder.Property(j => j.Id)
			.HasConversion(
				id => id.Value,
				value => JapanesePartOfSpeechId.From(value));

		// Code (from LinguisticMetadata base)
		builder.Property(j => j.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(j => j.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(j => j.Name)
			.HasMaxLength(200)
			.IsRequired();

		// VietnameseName (from LinguisticMetadata base)
		builder.Property(j => j.VietnameseName)
			.HasMaxLength(200)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(j => j.Description)
			.HasMaxLength(1000);
	}
}
