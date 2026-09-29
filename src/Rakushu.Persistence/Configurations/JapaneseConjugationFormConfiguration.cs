using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Persistence.Configurations;

internal sealed class JapaneseConjugationFormConfiguration : IEntityTypeConfiguration<JapaneseConjugationForm>
{
	public void Configure(EntityTypeBuilder<JapaneseConjugationForm> builder)
	{
		// Id
		builder.HasKey(jcf => jcf.Id);
		builder.Property(jcf => jcf.Id)
			.HasConversion(
				id => id.Value,
				value => JapaneseConjugationFormId.From(value));

		// Code (from LinguisticMetadata base)
		builder.Property(jcf => jcf.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(jcf => jcf.Code)
			.IsUnique();

		// Name (from LinguisticMetadata base)
		builder.Property(jcf => jcf.Name)
			.HasMaxLength(200)
			.IsRequired();

		// VietnameseName (from LinguisticMetadata base)
		builder.Property(jcf => jcf.VietnameseName)
			.HasMaxLength(200)
			.IsRequired();

		// Description (from LinguisticMetadata base)
		builder.Property(jcf => jcf.Description)
			.HasMaxLength(1000);
	}
}
