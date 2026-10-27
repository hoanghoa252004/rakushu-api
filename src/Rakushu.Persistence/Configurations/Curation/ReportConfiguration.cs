using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;

namespace Rakushu.Persistence.Configurations.Curation;

internal sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
	public void Configure(EntityTypeBuilder<Report> builder)
	{
		builder.ToTable("reports", "curation");

		builder.HasKey(r => r.Id);
		builder.Property(r => r.Id)
			.HasConversion(
				id => id.Value,
				value => ReportId.From(value));

		builder.Property(r => r.TargetType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(r => r.TargetId)
			.IsRequired();

		builder.Property(r => r.Category)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(r => r.Status)
			.HasMaxLength(20)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(r => r.CuratorNotes);

		builder.Property(r => r.ResolvedAt);

		builder.Property(r => r.CreatedAt)
			.IsRequired();

		builder.Property(r => r.UpdatedAt)
			.IsRequired();

		builder.HasMany(r => r.Details)
			.WithOne(d => d.Report)
			.HasForeignKey(d => d.ReportId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.HasIndex(r => new { r.TargetType, r.TargetId, r.Category });
		builder.HasIndex(r => r.Status);
	}
}
