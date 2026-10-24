using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
	public void Configure(EntityTypeBuilder<QuizAttempt> builder)
	{
		// Schema, Table & Constraints
		builder.ToTable("quiz_attempts", "srs", table =>
		{
			table.HasCheckConstraint("ck_quiz_attempts_counts", "total_items >= 0 AND answered_items >= 0 AND correct_items >= 0 AND correct_items <= answered_items AND answered_items <= total_items");
			table.HasCheckConstraint("ck_quiz_attempts_score", "score IS NULL OR (score >= 0 AND score <= 100)");
			table.HasCheckConstraint("ck_quiz_attempts_status", "status IN ('Created', 'InProgress', 'Completed', 'Abandoned')");
			table.HasCheckConstraint("ck_quiz_attempts_completed_at", "status <> 'Completed' OR completed_at IS NOT NULL");
		});

		// Id
		builder.HasKey(attempt => attempt.Id);
		builder.Property(attempt => attempt.Id)
			.HasConversion(id => id.Value, value => QuizAttemptId.From(value));

		// QuizId
		builder.Property(attempt => attempt.QuizId)
			.HasConversion(id => id.Value, value => QuizId.From(value))
			.IsRequired();

		// UserId
		builder.Property(attempt => attempt.UserId)
			.HasConversion(id => id.Value, value => UserId.From(value))
			.IsRequired();

		// Status
		builder.Property(attempt => attempt.Status)
			.HasConversion(status => status.ToString(), value => Enum.Parse<QuizAttemptStatus>(value))
			.HasMaxLength(20)
			.HasDefaultValue(QuizAttemptStatus.InProgress)
			.IsRequired();

		// Score & Counts
		builder.Property(attempt => attempt.Score)
			.HasPrecision(5, 2);

		builder.Property(attempt => attempt.TotalItems).IsRequired();
		builder.Property(attempt => attempt.AnsweredItems).IsRequired();
		builder.Property(attempt => attempt.CorrectItems).IsRequired();

		// Timestamps
		builder.Property(attempt => attempt.StartedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		builder.Property(attempt => attempt.CompletedAt)
			.HasColumnType("timestamp with time zone");

		builder.Property(attempt => attempt.CreatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		builder.Property(attempt => attempt.UpdatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		// Relationships
		builder.HasOne(attempt => attempt.User)
			.WithMany()
			.HasForeignKey(attempt => attempt.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(attempt => attempt.Quiz)
			.WithMany(quiz => quiz.Attempts)
			.HasForeignKey(attempt => attempt.QuizId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(attempt => attempt.Items)
			.WithOne(item => item.QuizAttempt)
			.HasForeignKey(item => item.QuizAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		// Indexes
		builder.HasIndex(attempt => new { attempt.UserId, attempt.StartedAt });
		builder.HasIndex(attempt => attempt.QuizId);
	}
}
