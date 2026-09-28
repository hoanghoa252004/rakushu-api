using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.OovCandidate;

namespace Rakushu.Persistence.Configurations;

internal sealed class OovCandidateConfiguration : IEntityTypeConfiguration<OovCandidate>
{
	public void Configure(EntityTypeBuilder<OovCandidate> builder)
	{
		builder.ToTable("oov_candidates");

		builder.HasKey(o => o.Id);
		builder.Property(o => o.Id)
			.HasConversion(
				id => id.Value,
				value => OovCandidateId.From(value))
			.HasColumnName("oov_candidate_id");

		builder.Property(o => o.TokenId)
			.HasColumnName("token_id");

		builder.Property(o => o.Term)
			.HasMaxLength(255)
			.HasColumnName("term")
			.IsRequired();

		builder.HasIndex(o => o.Term);

		builder.Property(o => o.TentativeReading)
			.HasMaxLength(255)
			.HasColumnName("tentative_reading");

		builder.Property(o => o.TentativePos)
			.HasMaxLength(100)
			.HasColumnName("tentative_pos");

		builder.Property(o => o.SuggestedMeaning)
			.HasColumnName("suggested_meaning");

		builder.Property(o => o.ContextSnippet)
			.HasColumnName("context_snippet");

		builder.Property(o => o.ConfidenceScore)
			.HasColumnName("confidence_score")
			.IsRequired();

		builder.Property(o => o.Status)
			.HasMaxLength(50)
			.HasConversion<string>()
			.HasColumnName("status")
			.IsRequired();

		builder.HasIndex(o => o.Status);

		builder.Property(o => o.DetectedAt)
			.HasColumnName("detected_at")
			.IsRequired();

		builder.Property(o => o.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();
	}
}
