using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
{
	public void Configure(EntityTypeBuilder<Subtitle> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(si => si.Id);
		builder.Property(si => si.Id)
			.HasConversion(
				id => id.Value,
				value => SubtitleId.From(value));

		// TranscriptSegmentId
		builder.Property(si => si.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(si => si.TranscriptSegment)
			.WithOne(ts => ts.Subtitle)
			.HasForeignKey<Subtitle>(si => si.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(si => si.TranscriptSegmentId)
			.IsUnique();

		// Text
		builder.Property(si => si.Text)
			.IsRequired();

		// Sequence
		builder.Property(si => si.Sequence)
			.IsRequired();

		// LanguageCode
		builder.Property(v => v.LanguageCode)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

	}
}
