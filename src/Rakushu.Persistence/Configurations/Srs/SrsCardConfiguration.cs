using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsCardConfiguration : IEntityTypeConfiguration<SrsCard>
{
	public void Configure(EntityTypeBuilder<SrsCard> builder)
	{

		// Table and constraints
		builder.ToTable("srs_cards", "srs", table =>
		{
			table.HasCheckConstraint(
				"ck_srs_cards_status",
				"status IN ('New', 'Learning', 'Review', 'Suspended', 'Archived')");
		});

		// Id
		builder.HasKey(card => card.Id);

		// Status
		builder.Property(card => card.Status)
			.HasConversion(
				status => status.ToString(),
				value => Enum.Parse<SrsCardStatus>(value))
			.HasMaxLength(20)
			.HasDefaultValue(SrsCardStatus.New)
			.IsRequired();

		// Relationships
		builder.HasOne(card => card.SrsNote)
			.WithOne()
			.HasForeignKey<SrsCard>(card => card.SrsNoteId)
			.OnDelete(DeleteBehavior.Cascade);

		// Indexes
		builder.HasIndex(card => card.SrsNoteId)
			.IsUnique();

		builder.HasIndex(card => card.Status);
	}
}