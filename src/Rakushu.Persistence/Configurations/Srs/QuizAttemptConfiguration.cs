using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
	public void Configure(EntityTypeBuilder<QuizAttempt> builder)
	{
		builder.ToTable("quiz_attempts", "srs");

		builder.HasKey(qa => qa.Id);
		builder.Property(qa => qa.Id)
			.HasConversion(
				id => id.Value,
				value => QuizAttemptId.From(value));

		builder.Property(qa => qa.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne(qa => qa.User)
			.WithMany()
			.HasForeignKey(qa => qa.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(qa => qa.QuizId)
			.HasConversion(
				id => id.Value,
				value => QuizId.From(value))
			.IsRequired();

		builder.HasOne(qa => qa.Quiz)
			.WithMany()
			.HasForeignKey(qa => qa.QuizId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(qa => qa.Status)
			.HasMaxLength(20)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(qa => qa.Mode)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(qa => qa.TotalItems)
			.HasColumnName("total_items")
			.IsRequired();

		builder.Property(qa => qa.AnsweredItems)
			.HasColumnName("answered_items")
			.IsRequired();

		builder.Property(qa => qa.CorrectItems)
			.HasColumnName("correct_items")
			.IsRequired();

		builder.Property(qa => qa.Score)
			.HasColumnName("score")
			.HasPrecision(5, 2);

		builder.Property(qa => qa.ExpiresAt)
			.HasColumnName("expires_at")
			.IsRequired();

		builder.Property(qa => qa.IsEvaluated)
			.HasColumnName("is_evaluated")
			.HasDefaultValue(false)
			.IsRequired();

		builder.Property(qa => qa.CompletedAt)
			.HasColumnName("completed_at");

		builder.Property(qa => qa.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(qa => qa.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();

		builder.HasMany(qa => qa.Items)
			.WithOne(qai => qai.QuizAttempt)
			.HasForeignKey(qai => qai.QuizAttemptId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
