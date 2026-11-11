using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment.ShadowingAssessmentDetail;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingAssessmentDetailConfiguration : IEntityTypeConfiguration<ShadowingAssessmentDetail>
{
	public void Configure(EntityTypeBuilder<ShadowingAssessmentDetail> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingAssessmentDetailId.From(value));

		// ShadowingAssessmentId
		builder.Property(d => d.ShadowingAssessmentId)
			.HasConversion(
				id => id.Value,
				value => ShadowingAssessmentId.From(value))
			.IsRequired();
		builder.HasOne(d => d.ShadowingAssessment)
			.WithMany(a => a.Details)
			.HasForeignKey(d => d.ShadowingAssessmentId)
			.OnDelete(DeleteBehavior.Cascade);

		// ShadowingAttemptDetailId
		builder.Property(d => d.ShadowingAttemptDetailId)
			.HasConversion(
				id => id.Value,
				value => ShadowingAttemptDetailId.From(value))
			.IsRequired();
		builder.HasOne(d => d.ShadowingAttemptDetail)
			.WithMany(ad => ad.AssessmentDetails)
			.HasForeignKey(d => d.ShadowingAttemptDetailId)
			.OnDelete(DeleteBehavior.Restrict);

		// One detail per attempt detail in an assessment
		builder.HasIndex(d => new { d.ShadowingAssessmentId, d.ShadowingAttemptDetailId })
			.IsUnique();

		// Scores
		builder.Property(d => d.AccuracyScore)
			.HasPrecision(5, 2);
		builder.Property(d => d.PitchSimilarityScore)
			.HasPrecision(5, 2);
		builder.Property(d => d.AccuracyAssessmentResponse);
		builder.Property(d => d.PitchSimilarityAssessmentResponse);

		// Feedback
		builder.Property(d => d.Feedback);
	}
}
