using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptItemConfiguration : IEntityTypeConfiguration<QuizAttemptItem>
{
	public void Configure(EntityTypeBuilder<QuizAttemptItem> builder)
	{
		builder.ToTable("quiz_attempt_items", "srs");

		builder.HasKey(qai => qai.Id);
		builder.Property(qai => qai.Id)
			.HasConversion(
				id => id.Value,
				value => QuizAttemptItemId.From(value));

		builder.Property(qai => qai.QuizAttemptId)
			.HasConversion(
				id => id.Value,
				value => QuizAttemptId.From(value))
			.IsRequired();

		builder.Property(qai => qai.QuestionId)
			.HasConversion(
				id => id.Value,
				value => QuestionId.From(value))
			.IsRequired();

		builder.HasOne(qai => qai.Question)
			.WithMany()
			.HasForeignKey(qai => qai.QuestionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(qai => qai.Position)
			.IsRequired();

		builder.Property(qai => qai.SelectedOptionId)
			.HasMaxLength(50);

		builder.Property(qai => qai.IsCorrect);

		builder.Property(qai => qai.ElapsedMs)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(qai => qai.Feedback);

		builder.Property(qai => qai.AnsweredAt);

		builder.Property(qai => qai.CreatedAt)
			.IsRequired();
	}
}
