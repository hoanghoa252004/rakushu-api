using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
	public void Configure(EntityTypeBuilder<QuizAttempt> builder)
	{
		// Table and constraints
		builder.ToTable("quiz_attempts", "srs", table =>
		{
			table.HasCheckConstraint(
				"ck_quiz_attempts_counts",
				"total_items >= 0 AND answered_items >= 0 AND correct_items >= 0 " +
				"AND correct_items <= answered_items AND answered_items <= total_items");
			table.HasCheckConstraint(
				"ck_quiz_attempts_score",
				"score IS NULL OR (score >= 0 AND score <= 100)");
			table.HasCheckConstraint(
				"ck_quiz_attempts_elapsed_ms",
				"elapsed_ms IS NULL OR elapsed_ms >= 0");
			table.HasCheckConstraint(
				"ck_quiz_attempts_status",
				"status IN ('Created', 'InProgress', 'Completed', 'Abandoned')");
			table.HasCheckConstraint(
				"ck_quiz_attempts_mode",
				"attempt_mode IN ('SrsReview', 'Practice')");
			table.HasCheckConstraint(
				"ck_quiz_attempts_completed_at",
				"status <> 'Completed' OR completed_at IS NOT NULL");
		});

		// Id
		builder.HasKey(attempt => attempt.Id);

		// UserId
		builder.Property(attempt => attempt.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		// TemplateCode
		builder.Property(attempt => attempt.TemplateCode)
			.HasMaxLength(100)
			.HasDefaultValue("WORD_TO_MEANING_MCQ")
			.IsRequired();

		// AttemptMode
		builder.Property(attempt => attempt.AttemptMode)
			.HasMaxLength(20)
			.HasDefaultValue("Practice")
			.IsRequired();

		// Status
		builder.Property(attempt => attempt.Status)
			.HasConversion(
				status => status.ToString(),
				value => Enum.Parse<QuizAttemptStatus>(value))
			.HasMaxLength(20)
			.HasDefaultValue(QuizAttemptStatus.Created)
			.IsRequired();

		// Score
		builder.Property(attempt => attempt.Score)
			.HasPrecision(5, 2);

		// Relationships
		builder.HasOne(attempt => attempt.User)
			.WithMany()
			.HasForeignKey(attempt => attempt.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(attempt => attempt.SourceAttempt)
			.WithMany()
			.HasForeignKey(attempt => attempt.SourceAttemptId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(attempt => attempt.Items)
			.WithOne(item => item.QuizAttempt)
			.HasForeignKey(item => item.QuizAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		// Indexes
		builder.HasIndex(attempt => new { attempt.UserId, attempt.StartedAt });

		builder.HasIndex(attempt => attempt.SourceAttemptId);
	}
}