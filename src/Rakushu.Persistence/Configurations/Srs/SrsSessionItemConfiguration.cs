using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsSessionItemConfiguration : IEntityTypeConfiguration<SrsSessionItem>
{
	public void Configure(EntityTypeBuilder<SrsSessionItem> builder)
	{
		// Table and constraints
		builder.ToTable("srs_session_items", "srs", table =>
		{
			table.HasCheckConstraint(
				"ck_srs_session_items_position",
				"position > 0");
			table.HasCheckConstraint(
				"ck_srs_session_items_elapsed_ms",
				"elapsed_ms IS NULL OR elapsed_ms >= 0");
		});

		// Id
		builder.HasKey(item => item.Id);

		// Review rating
		builder.Property(item => item.Rating)
			.HasMaxLength(20);

		// Relationships
		builder.HasOne(item => item.SrsCard)
			.WithMany()
			.HasForeignKey(item => item.SrsCardId)
			.OnDelete(DeleteBehavior.Restrict);

		// Indexes
		builder.HasIndex(item => new { item.SrsSessionId, item.Position })
			.IsUnique();

		builder.HasIndex(item => item.SrsCardId);
	}
}