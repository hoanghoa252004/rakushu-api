using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptItemConfiguration : IEntityTypeConfiguration<QuizAttemptItem>
{
	public void Configure(EntityTypeBuilder<QuizAttemptItem> builder)
	{
		// Table and constraints
		builder.ToTable("quiz_attempt_items", "srs", table =>
		{
			table.HasCheckConstraint(
				"ck_quiz_attempt_items_position",
				"position > 0");
			table.HasCheckConstraint(
				"ck_quiz_attempt_items_elapsed_ms",
				"elapsed_ms IS NULL OR elapsed_ms >= 0");
		});

		// Id
		builder.HasKey(item => item.Id);

		// Question
		builder.Property(item => item.QuestionType)
			.HasMaxLength(50)
			.HasDefaultValue("WORD_TO_MEANING_MCQ")
			.IsRequired();

		builder.Property(item => item.QuestionSnapshot)
			.HasColumnType("jsonb")
			.IsRequired();

		builder.Property(item => item.ChoicesSnapshot)
			.HasColumnType("jsonb");

		builder.Property(item => item.CorrectAnswerSnapshot)
			.HasColumnType("jsonb")
			.IsRequired();

		builder.Property(item => item.SelectedAnswer)
			.HasColumnType("jsonb");

		builder.Property(item => item.FeedbackSnapshot)
			.HasColumnType("jsonb");

		// Relationships
		builder.HasOne(item => item.SrsCard)
			.WithMany()
			.HasForeignKey(item => item.SrsCardId)
			.OnDelete(DeleteBehavior.Restrict);

		// Indexes
		builder.HasIndex(item => new { item.QuizAttemptId, item.Position })
			.IsUnique();

		builder.HasIndex(item => item.SrsCardId);
	}
}