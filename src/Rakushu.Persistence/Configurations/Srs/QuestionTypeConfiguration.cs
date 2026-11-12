using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
{
	public void Configure(EntityTypeBuilder<QuestionType> builder)
	{
		builder.ToTable("question_types", "srs");

		builder.HasKey(qt => qt.Id);
		builder.Property(qt => qt.Id)
			.HasConversion(
				id => id.Value,
				value => QuestionTypeId.From(value));

		builder.Property(qt => qt.Code)
			.HasMaxLength(50)
			.IsRequired();

		builder.HasIndex(qt => qt.Code)
			.IsUnique();

		builder.Property(qt => qt.Name)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(qt => qt.Description)
			.HasMaxLength(500);

		builder.Property(qt => qt.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Property(qt => qt.CreatedAt)
			.IsRequired();

		builder.Property(qt => qt.UpdatedAt)
			.IsRequired();
	}
}
