using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Curation.IssueCategory;

namespace Rakushu.Persistence.Configurations.Curation;

internal sealed class IssueCategoryConfiguration : IEntityTypeConfiguration<IssueCategory>
{
	public void Configure(EntityTypeBuilder<IssueCategory> builder)
	{
		builder.ToTable("issue_categories", "curation");

		builder.HasKey(ic => ic.Id);
		builder.Property(ic => ic.Id)
			.HasConversion(
				id => id.Value,
				value => IssueCategoryId.From(value));

		builder.Property(ic => ic.TargetType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(ic => ic.TargetId)
			.IsRequired();

		builder.Property(ic => ic.Category)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(ic => ic.StartCharIndex);

		builder.Property(ic => ic.EndCharIndex);

		builder.Property(ic => ic.CurrentCount)
			.IsRequired()
			.HasDefaultValue(1);

		builder.Property(ic => ic.Threshold)
			.IsRequired()
			.HasDefaultValue(3);

		builder.Property(ic => ic.LastReportedAt)
			.IsRequired();

		builder.Property(ic => ic.CreatedAt)
			.IsRequired();

		builder.Property(ic => ic.UpdatedAt)
			.IsRequired();

		builder.HasIndex(ic => new { ic.TargetType, ic.TargetId, ic.Category })
			.HasDatabaseName("ix_issue_categories_non_oov_unique")
			.HasFilter("start_char_index IS NULL AND end_char_index IS NULL")
			.IsUnique();

		builder.HasIndex(ic => new { ic.TargetType, ic.TargetId, ic.Category, ic.StartCharIndex, ic.EndCharIndex })
			.HasDatabaseName("ix_issue_categories_oov_span_unique")
			.HasFilter("start_char_index IS NOT NULL AND end_char_index IS NOT NULL")
			.IsUnique();
	}
}
