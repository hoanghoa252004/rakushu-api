using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class BankQuestionConfiguration : IEntityTypeConfiguration<BankQuestion>
{
	public void Configure(EntityTypeBuilder<BankQuestion> builder)
	{
		builder.ToTable("bank_questions", "srs");

		builder.HasKey(bq => bq.Id);
		builder.Property(bq => bq.Id)
			.HasConversion(
				id => id.Value,
				value => BankQuestionId.From(value));

		builder.Property(bq => bq.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();

		builder.HasOne(bq => bq.KnowledgeMeaning)
			.WithMany()
			.HasForeignKey(bq => bq.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(bq => bq.QuestionTypeId)
			.HasConversion(
				id => id.Value,
				value => QuestionTypeId.From(value))
			.IsRequired();

		builder.HasOne(bq => bq.QuestionType)
			.WithMany()
			.HasForeignKey(bq => bq.QuestionTypeId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(bq => bq.Prompt)
			.IsRequired();

		builder.Property(bq => bq.ChoicesJson)
			.HasColumnType("jsonb")
			.IsRequired();

		builder.Property(bq => bq.CorrectChoiceId)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(bq => bq.Explanation);

		builder.Property(bq => bq.Difficulty)
			.HasMaxLength(20)
			.IsRequired()
			.HasDefaultValue("Medium");

		builder.Property(bq => bq.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Property(bq => bq.CreatedAt)
			.IsRequired();

		builder.Property(bq => bq.UpdatedAt)
			.IsRequired();

		builder.HasIndex(bq => bq.KnowledgeMeaningId);
		builder.HasIndex(bq => bq.QuestionTypeId);
	}
}
