using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsItemConfiguration : IEntityTypeConfiguration<SrsItem>
{
	public void Configure(EntityTypeBuilder<SrsItem> builder)
	{
		// Schema, Table & Constraints
		builder.ToTable("srs_items", "srs", table =>
			table.HasCheckConstraint("ck_srs_items_status", "status IN ('New', 'Learning', 'Review', 'Suspended', 'Ignored')"));

		// Id
		builder.HasKey(item => item.Id);
		builder.Property(item => item.Id)
			.HasConversion(id => id.Value, value => SrsItemId.From(value));

		// Foreign Keys
		builder.Property(item => item.UserId)
			.HasConversion(id => id.Value, value => UserId.From(value))
			.IsRequired();

		builder.Property(item => item.KnowledgeMeaningId)
			.HasConversion(id => id.Value, value => KnowledgeMeaningId.From(value))
			.IsRequired();

		builder.Property(item => item.SourceLearningUnitId)
			.HasConversion(
				id => id != null ? id.Value : (Guid?)null,
				value => value.HasValue ? LearningUnitId.From(value.Value) : null);

		// Vocabulary Snapshot
		builder.Property(item => item.Expression)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(item => item.Reading)
			.HasMaxLength(100);

		builder.Property(item => item.Meaning)
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(item => item.Description)
			.HasColumnType("text");

		// SRS Lifecycle & SM-2 Metrics
		builder.Property(item => item.Status)
			.HasConversion(status => status.ToString(), value => Enum.Parse<SrsItemStatus>(value))
			.HasMaxLength(20)
			.HasDefaultValue(SrsItemStatus.New)
			.IsRequired();

		builder.Property(item => item.DueAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		builder.Property(item => item.IntervalDays)
			.HasDefaultValue(0)
			.IsRequired();

		builder.Property(item => item.EasinessFactor)
			.HasColumnType("double precision")
			.HasDefaultValue(2.5)
			.IsRequired();

		builder.Property(item => item.Repetitions)
			.HasDefaultValue(0)
			.IsRequired();

		builder.Property(item => item.Lapses)
			.HasDefaultValue(0)
			.IsRequired();

		builder.Property(item => item.LastReviewAt)
			.HasColumnType("timestamp with time zone");

		// Timestamps
		builder.Property(item => item.CreatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		builder.Property(item => item.UpdatedAt)
			.HasColumnType("timestamp with time zone")
			.IsRequired();

		// Relationships
		builder.HasOne(item => item.User)
			.WithMany()
			.HasForeignKey(item => item.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(item => item.KnowledgeMeaning)
			.WithMany()
			.HasForeignKey(item => item.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(item => item.SourceLearningUnit)
			.WithMany()
			.HasForeignKey(item => item.SourceLearningUnitId)
			.OnDelete(DeleteBehavior.SetNull);

		// Indexes
		builder.HasIndex(item => new { item.UserId, item.KnowledgeMeaningId })
			.IsUnique();

		builder.HasIndex(item => new { item.UserId, item.Status, item.DueAt });
		builder.HasIndex(item => item.DueAt);
	}
}
