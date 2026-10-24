using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
	public void Configure(EntityTypeBuilder<Quiz> builder)
	{
		// Schema & Table
		builder.ToTable("quizzes", "srs");

		// Id
		builder.HasKey(q => q.Id);
		builder.Property(q => q.Id)
			.HasConversion(id => id.Value, value => QuizId.From(value));

		// UserId
		builder.Property(q => q.UserId)
			.HasConversion(id => id.Value, value => UserId.From(value))
			.IsRequired();
		builder.HasIndex(q => q.UserId);

		// TotalQuestions
		builder.Property(q => q.TotalQuestions)
			.IsRequired();

		// CreatedAt
		builder.Property(q => q.CreatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		// Relationships
		builder.HasOne(q => q.User)
			.WithMany()
			.HasForeignKey(q => q.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(q => q.Questions)
			.WithOne(qn => qn.Quiz)
			.HasForeignKey(qn => qn.QuizId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(q => q.Attempts)
			.WithOne(a => a.Quiz)
			.HasForeignKey(a => a.QuizId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
