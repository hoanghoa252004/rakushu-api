using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class TranscriptSegmentConfiguration : IEntityTypeConfiguration<TranscriptSegment>
{
	public void Configure(EntityTypeBuilder<TranscriptSegment> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

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

		// TranscriptSpeakerId
		builder.Property(ts => ts.TranscriptSpeakerId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSpeakerId.From(value))
			.IsRequired();
		builder.HasOne(ts => ts.TranscriptSpeaker)
			.WithMany(t => t.TranscriptSegments)
			.HasForeignKey(ts => ts.TranscriptSpeakerId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
