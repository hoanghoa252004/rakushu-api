using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		// Schema & Table
		builder.ToTable("questions", "srs");

		// Id
		builder.HasKey(q => q.Id);
		builder.Property(q => q.Id)
			.HasConversion(id => id.Value, value => QuestionId.From(value));

		// QuizId
		builder.Property(q => q.QuizId)
			.HasConversion(id => id.Value, value => QuizId.From(value))
			.IsRequired();

		// SrsItemId
		builder.Property(q => q.SrsItemId)
			.HasConversion(id => id.Value, value => SrsItemId.From(value))
			.IsRequired();

		// Position
		builder.Property(q => q.Position)
			.IsRequired();

		// QuestionType
		builder.Property(q => q.QuestionType)
			.HasMaxLength(50)
			.IsRequired();

		// Prompt
		builder.Property(q => q.Prompt)
			.HasColumnType("text")
			.IsRequired();

		// Reading
		builder.Property(q => q.Reading)
			.HasMaxLength(100);

		// Choices
		builder.Property(q => q.ChoicesJson)
			.HasColumnName("choices")
			.HasColumnType("jsonb")
			.IsRequired();

		// CorrectOptionId
		builder.Property(q => q.CorrectOptionId)
			.HasMaxLength(10)
			.IsRequired();

		// CreatedAt
		builder.Property(q => q.CreatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		// Relationships
		builder.HasOne(q => q.Quiz)
			.WithMany(quiz => quiz.Questions)
			.HasForeignKey(q => q.QuizId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(q => q.SrsItem)
			.WithMany()
			.HasForeignKey(q => q.SrsItemId)
			.OnDelete(DeleteBehavior.Restrict);

		// Indexes
		builder.HasIndex(q => new { q.QuizId, q.Position })
			.IsUnique();

		builder.HasIndex(q => q.SrsItemId);
	}
}
