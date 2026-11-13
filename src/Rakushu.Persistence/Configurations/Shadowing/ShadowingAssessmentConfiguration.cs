using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingAssessmentConfiguration : IEntityTypeConfiguration<ShadowingAssessment>
{
	public void Configure(EntityTypeBuilder<ShadowingAssessment> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingAssessmentId.From(value));

		// ShadowingAttemptId
		builder.Property(a => a.ShadowingAttemptId)
			.HasConversion(
				id => id.Value,
				value => ShadowingAttemptId.From(value))
			.IsRequired();
		builder.HasOne(a => a.ShadowingAttempt)
			.WithMany(t => t.Assessments)
			.HasForeignKey(a => a.ShadowingAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		// Type (enum)
		builder.Property(a => a.Type)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// OverallScore
		builder.Property(a => a.OverallScore)
			.HasPrecision(5, 2);

		// OveralFeedback
		builder.Property(a => a.OveralFeedback);

		// CreatedAt
		builder.Property(a => a.CreatedAt)
			.IsRequired();
	}
}
