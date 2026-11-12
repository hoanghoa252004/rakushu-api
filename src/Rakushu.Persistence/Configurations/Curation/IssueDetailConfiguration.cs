using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Persistence.Configurations.Curation;

internal sealed class IssueDetailConfiguration : IEntityTypeConfiguration<IssueDetail>
{
	public void Configure(EntityTypeBuilder<IssueDetail> builder)
	{
		builder.ToTable("issue_details", "curation");

		builder.HasKey(id => id.Id);
		builder.Property(id => id.Id)
			.HasConversion(
				id => id.Value,
				value => IssueDetailId.From(value));

		builder.Property(id => id.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne(id => id.User)
			.WithMany()
			.HasForeignKey(id => id.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(id => id.TargetType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(id => id.TargetId)
			.IsRequired();

		builder.Property(id => id.Category)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(id => id.LearnerFeedback)
			.HasMaxLength(1000);

		builder.Property(id => id.ReportId)
			.HasConversion(
				id => id != null ? id.Value : (Guid?)null,
				value => value.HasValue ? ReportId.From(value.Value) : null);

		builder.Property(id => id.CreatedAt)
			.IsRequired();

		builder.HasIndex(id => new { id.TargetType, id.TargetId, id.Category });
		builder.HasIndex(id => id.UserId);
		builder.HasIndex(id => id.ReportId);
	}
}
