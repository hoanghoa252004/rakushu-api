using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
	public void Configure(EntityTypeBuilder<Quiz> builder)
	{
		builder.ToTable("quizzes", "srs");

		builder.HasKey(q => q.Id);
		builder.Property(q => q.Id)
			.HasConversion(
				id => id.Value,
				value => QuizId.From(value));

		builder.Property(q => q.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne(q => q.User)
			.WithMany()
			.HasForeignKey(q => q.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(q => q.TotalQuestions)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(q => q.CreatedAt)
			.IsRequired();

		builder.HasMany(q => q.Questions)
			.WithOne(qn => qn.Quiz)
			.HasForeignKey(qn => qn.QuizId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
