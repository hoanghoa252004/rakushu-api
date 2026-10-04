using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaningDetail;
using Rakushu.Domain.Entities.SupportedLanguage;

namespace Rakushu.Persistence.Configurations;

internal sealed class KnowledgeMeaningDetailConfiguration : IEntityTypeConfiguration<KnowledgeMeaningDetail>
{
	public void Configure(EntityTypeBuilder<KnowledgeMeaningDetail> builder)
	{
		builder.ToTable("knowledge_meaning_detail");

		// Id
		builder.HasKey(kmd => kmd.Id);
		builder.Property(kmd => kmd.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningDetailId.From(value));

		// KnowledgeMeaningId
		builder.Property(kmd => kmd.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();
		builder.HasOne(kmd => kmd.KnowledgeMeaning)
			.WithMany(km => km.Details)
			.HasForeignKey(kmd => kmd.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Cascade);

		// SupportedLanguageId
		builder.Property(kmd => kmd.SupportedLanguageId)
			.HasConversion(
				id => id.Value,
				value => SupportedLanguageId.From(value))
			.IsRequired();
		builder.HasOne(kmd => kmd.SupportedLanguage)
			.WithMany(sl => sl.KnowledgeMeaningDetails)
			.HasForeignKey(kmd => kmd.SupportedLanguageId)
			.OnDelete(DeleteBehavior.Cascade);

		// TranslatedMeaning
		builder.Property(kmd => kmd.TranslatedMeaning)
			.HasMaxLength(500)
			.IsRequired();

		// NativeNote
		builder.Property(kmd => kmd.NativeNote)
			.HasMaxLength(500);

		// CreatedAt
		builder.Property(kmd => kmd.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(kmd => kmd.UpdatedAt)
			.IsRequired();
	}
}
