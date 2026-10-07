using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.CuratorReview;
using Rakushu.Domain.Entities.OovCandidate;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Configurations;

internal sealed class CuratorReviewConfiguration : IEntityTypeConfiguration<CuratorReview>
{
	public void Configure(EntityTypeBuilder<CuratorReview> builder)
	{
		builder.ToTable("curator_reviews");

		builder.HasKey(r => r.Id);
		builder.Property(r => r.Id)
			.HasConversion(
				id => id.Value,
				value => CuratorReviewId.From(value))
			.HasColumnName("review_id");

		builder.Property(r => r.OovCandidateId)
			.HasConversion(
				id => id.Value,
				value => OovCandidateId.From(value))
			.HasColumnName("oov_candidate_id")
			.IsRequired();

		builder.Property(r => r.CuratorId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.HasColumnName("curator_id")
			.IsRequired();

		builder.Property(r => r.Decision)
			.HasMaxLength(30)
			.HasConversion<string>()
			.HasColumnName("decision")
			.IsRequired();

		builder.Property(r => r.EditedTerm)
			.HasMaxLength(255)
			.HasColumnName("edited_term");

		builder.Property(r => r.EditedReading)
			.HasMaxLength(255)
			.HasColumnName("edited_reading");

		builder.Property(r => r.EditedPos)
			.HasMaxLength(100)
			.HasColumnName("edited_pos");

		builder.Property(r => r.EditedMeaning)
			.HasColumnName("edited_meaning");

		builder.Property(r => r.Comment)
			.HasColumnName("comment");

		builder.Property(r => r.ReviewedAt)
			.HasColumnName("reviewed_at")
			.IsRequired();

		builder.Property(r => r.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		// Relationships
		builder.HasOne(r => r.Curator)
			.WithMany()
			.HasForeignKey(r => r.CuratorId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(r => r.OovCandidate)
			.WithOne(c => c.CuratorReview)
			.HasForeignKey<CuratorReview>(r => r.OovCandidateId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(r => r.OovCandidateId)
			.IsUnique();
	}
}
