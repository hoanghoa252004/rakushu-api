using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionCard;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class FlashcardSessionCardConfiguration : IEntityTypeConfiguration<FlashcardSessionCard>
{
	public void Configure(EntityTypeBuilder<FlashcardSessionCard> builder)
	{
		builder.ToTable("flashcard_session_cards", "srs");

		builder.HasKey(fsc => new { fsc.FlashcardSessionId, fsc.SavedItemId });

		builder.Property(fsc => fsc.FlashcardSessionId)
			.HasConversion(
				id => id.Value,
				value => FlashcardSessionId.From(value))
			.IsRequired();

		builder.Property(fsc => fsc.SavedItemId)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value))
			.IsRequired();

		builder.HasOne(fsc => fsc.FlashcardSession)
			.WithMany(fs => fs.Cards)
			.HasForeignKey(fsc => fsc.FlashcardSessionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(fsc => fsc.SavedItem)
			.WithMany()
			.HasForeignKey(fsc => fsc.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
