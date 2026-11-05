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
			.IsRequired();

		builder.Property(qa => qa.AnsweredItems)
			.IsRequired();

		builder.Property(qa => qa.CorrectItems)
			.IsRequired();

		builder.Property(qa => qa.Score)
			.HasPrecision(5, 2);

		builder.Property(qa => qa.StartedAt)
			.IsRequired();

		builder.Property(qa => qa.CompletedAt);

		builder.Property(qa => qa.CreatedAt)
			.IsRequired();

		builder.Property(qa => qa.UpdatedAt)
			.IsRequired();

		builder.HasMany(qa => qa.Items)
			.WithOne(qai => qai.QuizAttempt)
			.HasForeignKey(qai => qai.QuizAttemptId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
