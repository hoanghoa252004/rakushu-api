using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptItemConfiguration : IEntityTypeConfiguration<QuizAttemptItem>
{
	public void Configure(EntityTypeBuilder<QuizAttemptItem> builder)
	{
		// Schema, Table & Constraints
		builder.ToTable("quiz_attempt_items", "srs", table =>
		{
			table.HasCheckConstraint("ck_quiz_attempt_items_position", "position > 0");
			table.HasCheckConstraint("ck_quiz_attempt_items_quality", "quality IS NULL OR (quality >= 0 AND quality <= 5)");
		});

		// Id
		builder.HasKey(item => item.Id);
		builder.Property(item => item.Id)
			.HasConversion(id => id.Value, value => QuizAttemptItemId.From(value));

		// Foreign Keys
		builder.Property(item => item.QuizAttemptId)
			.HasConversion(id => id.Value, value => QuizAttemptId.From(value))
			.IsRequired();

		builder.Property(item => item.QuestionId)
			.HasConversion(id => id.Value, value => QuestionId.From(value))
			.IsRequired();

		builder.Property(item => item.SrsItemId)
			.HasConversion(id => id.Value, value => SrsItemId.From(value))
			.IsRequired();

		// Question & Answer State
		builder.Property(item => item.Position)
			.IsRequired();

		builder.Property(item => item.SelectedOptionId)
			.HasMaxLength(10);

		builder.Property(item => item.IsCorrect);

		builder.Property(item => item.ElapsedMs)
			.HasDefaultValue(0)
			.IsRequired();

		builder.Property(item => item.Quality);

		builder.Property(item => item.Feedback)
			.HasColumnType("text");

		// Timestamps
		builder.Property(item => item.AnsweredAt)
			.HasColumnType("timestamp with time zone");

		builder.Property(item => item.CreatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		// Relationships
		builder.HasOne(item => item.QuizAttempt)
			.WithMany(attempt => attempt.Items)
			.HasForeignKey(item => item.QuizAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(item => item.Question)
			.WithMany()
			.HasForeignKey(item => item.QuestionId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(item => item.SrsItem)
			.WithMany()
			.HasForeignKey(item => item.SrsItemId)
			.OnDelete(DeleteBehavior.Restrict);

		// Indexes
		builder.HasIndex(item => new { item.QuizAttemptId, item.Position })
			.IsUnique();

		builder.HasIndex(item => item.QuestionId);
		builder.HasIndex(item => item.SrsItemId);
	}
}
