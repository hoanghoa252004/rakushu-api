using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.DictionaryEntry;

namespace Rakushu.Persistence.Configurations;

internal sealed class DictionaryEntryConfiguration : IEntityTypeConfiguration<DictionaryEntry>
{
	public void Configure(EntityTypeBuilder<DictionaryEntry> builder)
	{
		builder.ToTable("dictionary_entries");

		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.HasConversion(
				id => id.Value,
				value => DictionaryEntryId.From(value))
			.HasColumnName("dictionary_entry_id");

		builder.Property(d => d.Term)
			.HasMaxLength(255)
			.HasColumnName("term")
			.IsRequired();

		builder.HasIndex(d => d.Term);

		builder.Property(d => d.Reading)
			.HasMaxLength(255)
			.HasColumnName("reading")
			.IsRequired();

		builder.HasIndex(d => d.Reading);

		builder.Property(d => d.Pos)
			.HasMaxLength(100)
			.HasColumnName("part_of_speech")
			.IsRequired();

		builder.Property(d => d.Meaning)
			.HasColumnName("meaning")
			.IsRequired();

		builder.Property(d => d.MeaningVietnamese)
			.HasColumnName("meaning_vietnamese");

		builder.Property(d => d.JlptLevel)
			.HasMaxLength(10)
			.HasColumnName("jlpt_level");

		builder.Property(d => d.WordType)
			.HasMaxLength(50)
			.HasColumnName("word_type");

		builder.Property(d => d.AudioUrl)
			.HasMaxLength(500)
			.HasColumnName("audio_url");

		builder.Property(d => d.DefinitionTags)
			.HasMaxLength(255)
			.HasColumnName("definition_tags");

		builder.Property(d => d.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(d => d.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();
	}
}
