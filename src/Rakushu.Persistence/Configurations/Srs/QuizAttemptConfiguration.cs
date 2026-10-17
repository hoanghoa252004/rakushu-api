using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("quiz_attempts", "srs", table =>
        {
            table.HasCheckConstraint("ck_quiz_attempts_counts", "total_items >= 0 AND answered_items >= 0 AND correct_items >= 0 AND correct_items <= answered_items AND answered_items <= total_items");
            table.HasCheckConstraint("ck_quiz_attempts_score", "score IS NULL OR (score >= 0 AND score <= 100)");            table.HasCheckConstraint("ck_quiz_attempts_status", "status IN ('Created', 'InProgress', 'Completed', 'Abandoned')");
            table.HasCheckConstraint("ck_quiz_attempts_mode", "attempt_mode IN ('SrsReview', 'Practice')");
            table.HasCheckConstraint("ck_quiz_attempts_completed_at", "status <> 'Completed' OR completed_at IS NOT NULL");
        });

        builder.HasKey(attempt => attempt.Id);
        builder.Property(attempt => attempt.Id)
            .HasConversion(id => id.Value, value => QuizAttemptId.From(value));

        builder.Property(attempt => attempt.UserId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(attempt => attempt.TemplateCode).HasMaxLength(100).HasDefaultValue("WORD_TO_MEANING_MCQ").IsRequired();
        builder.Property(attempt => attempt.AttemptMode).HasMaxLength(20).HasDefaultValue("Practice").IsRequired();
        builder.Property(attempt => attempt.Status)
            .HasConversion(status => status.ToString(), value => Enum.Parse<QuizAttemptStatus>(value))
            .HasMaxLength(20)
            .HasDefaultValue(QuizAttemptStatus.Created)
            .IsRequired();
        builder.Property(attempt => attempt.Score).HasPrecision(5, 2);

        builder.HasOne(attempt => attempt.User).WithMany().HasForeignKey(attempt => attempt.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(attempt => attempt.SourceAttempt).WithMany().HasForeignKey(attempt => attempt.SourceAttemptId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(attempt => attempt.Items).WithOne(item => item.QuizAttempt).HasForeignKey(item => item.QuizAttemptId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(attempt => new { attempt.UserId, attempt.StartedAt });
        builder.HasIndex(attempt => attempt.SourceAttemptId);
    }
}