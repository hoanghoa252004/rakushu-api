using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class FlashcardSessionConfiguration : IEntityTypeConfiguration<FlashcardSession>
{
	public void Configure(EntityTypeBuilder<FlashcardSession> builder)
	{
		builder.ToTable("flashcard_sessions", "srs");

		builder.HasKey(fs => fs.Id);
		builder.Property(fs => fs.Id)
			.HasConversion(
				id => id.Value,
				value => FlashcardSessionId.From(value));

		builder.Property(fs => fs.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne(fs => fs.User)
			.WithMany()
			.HasForeignKey(fs => fs.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(fs => fs.Status)
			.HasMaxLength(20)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(fs => fs.CreatedAt)
			.IsRequired();

		builder.Property(fs => fs.ExpiresAt)
			.IsRequired();

		builder.Property(fs => fs.CompletedAt);

		builder.HasMany(fs => fs.Items)
			.WithOne(fsi => fsi.FlashcardSession)
			.HasForeignKey(fsi => fsi.FlashcardSessionId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
