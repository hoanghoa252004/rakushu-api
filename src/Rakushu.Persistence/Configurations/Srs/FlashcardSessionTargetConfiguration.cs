using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionTarget;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class FlashcardSessionTargetConfiguration : IEntityTypeConfiguration<FlashcardSessionTarget>
{
	public void Configure(EntityTypeBuilder<FlashcardSessionTarget> builder)
	{
		builder.ToTable("flashcard_session_targets", "srs");

		builder.HasKey(fst => new { fst.FlashcardSessionId, fst.SavedItemId });

		builder.Property(fst => fst.FlashcardSessionId)
			.HasConversion(
				id => id.Value,
				value => FlashcardSessionId.From(value))
			.IsRequired();

		builder.Property(fst => fst.SavedItemId)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value))
			.IsRequired();

		builder.HasOne(fst => fst.FlashcardSession)
			.WithMany(fs => fs.Targets)
			.HasForeignKey(fst => fst.FlashcardSessionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(fst => fst.SavedItem)
			.WithMany()
			.HasForeignKey(fst => fst.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
