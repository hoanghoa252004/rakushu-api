using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		builder.ToTable("questions", "srs");

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
			.HasConversion(
				id => id != null ? id.Value : (Guid?)null,
				value => value.HasValue ? BankQuestionId.From(value.Value) : null);

		builder.HasOne(qn => qn.BankQuestion)
			.WithMany()
			.HasForeignKey(qn => qn.BankQuestionId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.Property(qn => qn.QuestionTypeId)
			.HasConversion(
				id => id.Value,
				value => QuestionTypeId.From(value))
			.IsRequired();

		builder.HasOne(qn => qn.QuestionType)
			.WithMany()
			.HasForeignKey(qn => qn.QuestionTypeId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(qn => qn.Position)
			.IsRequired();

		builder.Property(qn => qn.Prompt)
			.IsRequired();

		builder.Property(qn => qn.Reading)
			.HasMaxLength(100);

		builder.Property(qn => qn.ChoicesJson)
			.HasColumnType("jsonb")
			.IsRequired();

		builder.Property(qn => qn.CorrectOptionId)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(qn => qn.CreatedAt)
			.IsRequired();
	}
}
