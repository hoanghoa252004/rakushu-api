using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsImmersionBoostLogConfiguration : IEntityTypeConfiguration<SrsImmersionBoostLog>
{
	public void Configure(EntityTypeBuilder<SrsImmersionBoostLog> builder)
	{
		builder.ToTable("srs_immersion_boost_logs", "srs");

		builder.HasKey(b => b.Id);
		builder.Property(b => b.Id)
			.HasConversion(
				id => id.Value,
				value => SrsImmersionBoostLogId.From(value));

		builder.Property(b => b.SrsScheduleId)
			.HasConversion(
				id => id.Value,
				value => SrsScheduleId.From(value))
			.IsRequired();

		builder.HasOne(b => b.SrsSchedule)
			.WithMany(s => s.BoostLogs)
			.HasForeignKey(b => b.SrsScheduleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(b => b.AdjustmentType)
			.HasMaxLength(32)
			.IsRequired();

		builder.Property(b => b.BonusDays)
			.IsRequired();

		builder.Property(b => b.PreviousDueAt)
			.IsRequired();

		builder.Property(b => b.NewDueAt)
			.IsRequired();

		builder.Property(b => b.BoostNumber)
			.IsRequired();

		builder.Property(b => b.EncounterCountInWindow)
			.IsRequired();

		builder.Property(b => b.BoostedAt)
			.IsRequired();

		builder.HasIndex(b => new { b.SrsScheduleId, b.BoostedAt });
	}
}
