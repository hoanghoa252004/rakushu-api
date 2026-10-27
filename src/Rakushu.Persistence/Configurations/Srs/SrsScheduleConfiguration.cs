using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsScheduleConfiguration : IEntityTypeConfiguration<SrsSchedule>
{
	public void Configure(EntityTypeBuilder<SrsSchedule> builder)
	{
		builder.ToTable("srs_schedules", "srs");

		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => SrsScheduleId.From(value));

		builder.Property(s => s.SavedItemId)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value))
			.IsRequired();

		builder.HasIndex(s => s.SavedItemId)
			.IsUnique();

		builder.Property(s => s.Status)
			.HasMaxLength(20)
			.HasConversion<string>()
			.IsRequired();

		builder.Property(s => s.DueAt)
			.IsRequired();

		builder.Property(s => s.LastReviewedAt);

		builder.Property(s => s.CreatedAt)
			.IsRequired();

		builder.Property(s => s.UpdatedAt)
			.IsRequired();

		builder.HasIndex(s => new { s.Status, s.DueAt });
	}
}
