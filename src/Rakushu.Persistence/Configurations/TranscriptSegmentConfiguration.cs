using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations;

internal sealed class TranscriptSegmentConfiguration : IEntityTypeConfiguration<TranscriptSegment>
{
	public void Configure(EntityTypeBuilder<TranscriptSegment> builder)
	{
		// Id
		builder.HasKey(ts => ts.Id);
		builder.Property(ts => ts.Id)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value));

		// Sequence
		builder.Property(ts => ts.Sequence)
			.IsRequired();

		// Text
		builder.Property(ts => ts.Text)
			.IsRequired();

		// StartTime
		builder.Property(ts => ts.StartTime)
			.IsRequired();

		// EndTime
		builder.Property(ts => ts.EndTime)
			.IsRequired();

		// TranscriptId
		builder.Property(ts => ts.TranscriptId)
			.HasConversion(
				id => id.Value,
				value => TranscriptId.From(value))
			.IsRequired();
		builder.HasOne(ts => ts.Transcript)
			.WithMany(t => t.TranscriptSegments)
			.HasForeignKey(ts => ts.TranscriptId)
			.OnDelete(DeleteBehavior.Cascade);

		// CreatedAt
		builder.Property(ts => ts.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(ts => ts.UpdatedAt)
			.IsRequired();
	}
}
