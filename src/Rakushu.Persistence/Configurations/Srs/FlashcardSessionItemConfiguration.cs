using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class FlashcardSessionItemConfiguration : IEntityTypeConfiguration<FlashcardSessionItem>
{
	public void Configure(EntityTypeBuilder<FlashcardSessionItem> builder)
	{
		builder.ToTable("flashcard_session_items", "srs");

		builder.HasKey(fsi => fsi.Id);
		builder.Property(fsi => fsi.Id)
			.HasConversion(
				id => id.Value,
				value => FlashcardSessionItemId.From(value));

		builder.Property(fsi => fsi.FlashcardSessionId)
			.HasConversion(
				id => id.Value,
				value => FlashcardSessionId.From(value))
			.IsRequired();

		builder.Property(fsi => fsi.SavedItemId)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value))
			.IsRequired();

		builder.HasOne(fsi => fsi.SavedItem)
			.WithMany()
			.HasForeignKey(fsi => fsi.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(fsi => fsi.Expression)
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(fsi => fsi.Reading)
			.HasMaxLength(255);

		builder.Property(fsi => fsi.Meaning)
			.HasMaxLength(1000)
			.IsRequired();

		builder.Property(fsi => fsi.VideoTitle)
			.HasMaxLength(255);

		builder.Property(fsi => fsi.StorageKey)
			.HasMaxLength(500);

		builder.Property(fsi => fsi.VideoSourceUrl)
			.HasMaxLength(1000);

		builder.Property(fsi => fsi.StartTime);

		builder.Property(fsi => fsi.EndTime);

		builder.Property(fsi => fsi.OriginalSentence)
			.HasColumnType("text");

		builder.Property(fsi => fsi.TranslatedSentence)
			.HasColumnType("text");

		builder.Property(fsi => fsi.StartTokenIndex);

		builder.Property(fsi => fsi.EndTokenIndex);

		builder.Property(fsi => fsi.Rating)
			.HasMaxLength(20)
			.HasConversion<string>();

		builder.Property(fsi => fsi.ReviewedAt);
	}
}
