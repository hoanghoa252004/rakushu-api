using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Persistence.Configurations;

internal sealed class KnowledgePatternElementConfiguration : IEntityTypeConfiguration<KnowledgePatternElement>
{
	public void Configure(EntityTypeBuilder<KnowledgePatternElement> builder)
	{
		// Id
		builder.HasKey(kpe => kpe.Id);
		builder.Property(kpe => kpe.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternElementId.From(value));

		// KnowledgePatternId
		builder.Property(kpe => kpe.KnowledgePatternId)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternId.From(value))
			.IsRequired();
		builder.HasOne(kpe => kpe.KnowledgePattern)
			.WithMany(kp => kp.Elements)
			.HasForeignKey(kpe => kpe.KnowledgePatternId)
			.OnDelete(DeleteBehavior.Cascade);

		// Sequence
		builder.Property(kpe => kpe.Sequence)
			.IsRequired();

		// Role
		builder.Property(kpe => kpe.Role)
			.HasMaxLength(100)
			.IsRequired();

		// Scope
		builder.Property(kpe => kpe.Scope)
			.HasMaxLength(100);

		// Surface
		builder.Property(kpe => kpe.Surface)
			.HasMaxLength(255);

		// Lemma
		builder.Property(kpe => kpe.Lemma)
			.HasMaxLength(255);

		// UniversalPartOfSpeechId
		builder.Property(kpe => kpe.UniversalPartOfSpeechId)
			.HasConversion(
				id => id.Value,
				value => UniversalPartOfSpeechId.From(value));
		builder.HasOne(x => x.UniversalPartOfSpeech)
			.WithMany(up => up.KnowledgePatternElements)
			.HasForeignKey(kpe => kpe.UniversalPartOfSpeechId)
			.OnDelete(DeleteBehavior.Restrict);

		// JapanesePartOfSpeechId
		builder.Property(kpe => kpe.JapanesePartOfSpeechId)
			.HasConversion(
				id => id.Value,
				value => JapanesePartOfSpeechId.From(value));
		builder.HasOne(x => x.JapanesePartOfSpeech)
			.WithMany(xx => xx.KnowledgePatternElements)
			.HasForeignKey(kpe => kpe.JapanesePartOfSpeechId)
			.OnDelete(DeleteBehavior.Restrict);

		// JapaneseConjugationFormId
		builder.Property(kpe => kpe.JapaneseConjugationFormId)
			.HasConversion(
				id => id.Value,
				value => JapaneseConjugationFormId.From(value));
		builder.HasOne(x => x.JapaneseConjugationForm)
			.WithMany(xx => xx.KnowledgePatternElements)
			.HasForeignKey(kpe => kpe.JapaneseConjugationFormId)
			.OnDelete(DeleteBehavior.Restrict);

		// Particle
		builder.Property(kpe => kpe.Particle)
			.HasMaxLength(10);

		// SlotName
		builder.Property(kpe => kpe.SlotName)
			.HasMaxLength(100);

		// CreatedAt
		builder.Property(kpe => kpe.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(kpe => kpe.UpdatedAt)
			.IsRequired();
	}
}
