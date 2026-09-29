using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Video.Subtitle;
using Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations;

internal sealed class SubtitleSegmentConfiguration : IEntityTypeConfiguration<SubtitleSegment>
{
	public void Configure(EntityTypeBuilder<SubtitleSegment> builder)
	{
		// Id
		builder.HasKey(si => si.Id);
		builder.Property(si => si.Id)
			.HasConversion(
				id => id.Value,
				value => SubtitleSegmentId.From(value));

		// SubtitleId
		builder.Property(si => si.SubtitleId)
			.HasConversion(
				id => id.Value,
				value => SubtitleId.From(value))
			.IsRequired();
		builder.HasOne(si => si.Subtitle)
			.WithMany(s => s.Items)
			.HasForeignKey(si => si.SubtitleId)
			.OnDelete(DeleteBehavior.Cascade);

		// TranscriptSegmentId
		builder.Property(si => si.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(si => si.TranscriptSegment)
			.WithOne(ts => ts.SubtitleSegment)
			.HasForeignKey<SubtitleSegment>(si => si.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(si => si.TranscriptSegmentId)
			.IsUnique();

		// OriginalText
		builder.Property(si => si.OriginalText)
			.IsRequired();

		// TranslatedText
		builder.Property(si => si.TranslatedText)
			.IsRequired();

		// Sequence
		builder.Property(si => si.Sequence)
			.IsRequired();

		// StartTime
		builder.Property(si => si.StartTime)
			.IsRequired();

		// EndTime
		builder.Property(si => si.EndTime)
			.IsRequired();

		// CreatedAt
		builder.Property(si => si.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(si => si.UpdatedAt)
			.IsRequired();
	}
}
