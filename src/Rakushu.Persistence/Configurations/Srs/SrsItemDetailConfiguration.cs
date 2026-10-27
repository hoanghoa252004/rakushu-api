using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsItemDetailConfiguration : IEntityTypeConfiguration<SrsItemDetail>
{
	public void Configure(EntityTypeBuilder<SrsItemDetail> builder)
	{
		builder.ToTable("srs_item_details", "srs");

		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.HasConversion(
				id => id.Value,
				value => SrsItemDetailId.From(value));

		builder.Property(d => d.SrsScheduleId)
			.HasConversion(
				id => id.Value,
				value => SrsScheduleId.From(value))
			.IsRequired();

		builder.HasOne(d => d.SrsSchedule)
			.WithMany(s => s.Details)
			.HasForeignKey(d => d.SrsScheduleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(d => d.QuizAttemptItemId)
			.HasConversion(
				id => id != null ? id.Value : (Guid?)null,
				value => value.HasValue ? QuizAttemptItemId.From(value.Value) : null);

		builder.HasOne(d => d.QuizAttemptItem)
			.WithMany()
			.HasForeignKey(d => d.QuizAttemptItemId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.Property(d => d.ReviewNumber)
			.IsRequired();

		builder.Property(d => d.Quality)
			.IsRequired();

		builder.Property(d => d.IntervalDays)
			.IsRequired();

		builder.Property(d => d.EasinessFactor)
			.IsRequired();

		builder.Property(d => d.Repetitions)
			.IsRequired();

		builder.Property(d => d.Lapses)
			.IsRequired();

		builder.Property(d => d.ReviewedAt)
			.IsRequired();

		builder.HasIndex(d => new { d.SrsScheduleId, d.ReviewedAt });
	}
}
