using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptItemConfiguration : IEntityTypeConfiguration<QuizAttemptItem>
{
    public void Configure(EntityTypeBuilder<QuizAttemptItem> builder)
    {
        builder.ToTable("quiz_attempt_items", "srs", table =>
        {
            table.HasCheckConstraint("ck_quiz_attempt_items_position", "position > 0");
            table.HasCheckConstraint("ck_quiz_attempt_items_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
        });
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasConversion(id => id.Value, value => QuizAttemptItemId.From(value));
        builder.Property(item => item.QuizAttemptId).HasConversion(id => id.Value, value => QuizAttemptId.From(value));
        builder.Property(item => item.SrsCardId).HasConversion(id => id.Value, value => SrsCardId.From(value));
        builder.Property(item => item.QuestionType).HasMaxLength(50).HasDefaultValue("WORD_TO_MEANING_MCQ").IsRequired();
        builder.Property(item => item.QuestionSnapshot).HasColumnType("jsonb").IsRequired();
        builder.Property(item => item.ChoicesSnapshot).HasColumnType("jsonb");
        builder.Property(item => item.CorrectAnswerSnapshot).HasColumnType("jsonb").IsRequired();
        builder.Property(item => item.SelectedAnswer).HasColumnType("jsonb");
        builder.Property(item => item.FeedbackSnapshot).HasColumnType("jsonb");
        builder.HasOne(item => item.QuizAttempt).WithMany(attempt => attempt.Items).HasForeignKey(item => item.QuizAttemptId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.SrsCard).WithMany().HasForeignKey(item => item.SrsCardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(item => new { item.QuizAttemptId, item.Position }).IsUnique();
        builder.HasIndex(item => item.SrsCardId);
    }
}
