using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		builder.ToTable("quiz_items", "srs");

		builder.HasKey(qn => qn.Id);
		builder.Property(qn => qn.Id)
			.HasConversion(
				id => id.Value,
				value => QuestionId.From(value));

		builder.Property(qn => qn.QuizId)
			.HasConversion(
				id => id.Value,
				value => QuizId.From(value))
			.IsRequired();

		builder.Property(qn => qn.SavedItemId)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value))
			.IsRequired();

		builder.HasOne(qn => qn.SavedItem)
			.WithMany()
			.HasForeignKey(qn => qn.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(qn => qn.BankQuestionId)
			.HasColumnName("question_id")
			.HasConversion(
				id => id.Value,
				value => BankQuestionId.From(value))
			.IsRequired();

		builder.HasOne(qn => qn.BankQuestion)
			.WithMany()
			.HasForeignKey(qn => qn.BankQuestionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(qn => qn.Sequence)
			.HasColumnName("sequence")
			.IsRequired();

		builder.Property(qn => qn.QuestionText)
			.HasColumnName("question")
			.IsRequired();

		builder.Property(qn => qn.Answers)
			.HasColumnName("answers")
			.HasColumnType("jsonb")
			.IsRequired();

		builder.Property(qn => qn.CorrectAnswer)
			.HasColumnName("correct_answer")
			.HasMaxLength(500)
			.IsRequired();

		builder.Property(qn => qn.MediaType)
			.HasColumnName("media_type")
			.HasMaxLength(20)
			.IsRequired()
			.HasDefaultValue("NONE");

		builder.Property(qn => qn.MediaUrl)
			.HasColumnName("media_url");

		builder.Property(qn => qn.MediaMetadata)
			.HasColumnName("media_metadata")
			.HasColumnType("jsonb");
	}
}
