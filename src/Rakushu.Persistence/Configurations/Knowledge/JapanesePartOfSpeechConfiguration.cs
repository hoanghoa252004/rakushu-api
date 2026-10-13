using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class JapanesePartOfSpeechConfiguration : IEntityTypeConfiguration<JapanesePartOfSpeech>
{
	public void Configure(EntityTypeBuilder<JapanesePartOfSpeech> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(j => j.Id);
		builder.Property(j => j.Id)
			.HasConversion(
				id => id.Value,
				value => JapanesePartOfSpeechId.From(value));

		// Code (from LinguisticMetadata base)
		builder.Property(pl => pl.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(j => j.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName (from LinguisticMetadata base)
		builder.Property(j => j.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(j => j.Description)
			.HasMaxLength(1000);
	}
}
