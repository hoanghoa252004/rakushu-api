using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class TranscriptItemConfiguration : IEntityTypeConfiguration<TranscriptItem>
{
	public void Configure(EntityTypeBuilder<TranscriptItem> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(ts => ts.Id);
		builder.Property(ts => ts.Id)
			.HasConversion(
				id => id.Value,
				value => TranscriptItemId.From(value));

		// Sequence
		builder.Property(ts => ts.Sequence)
			.IsRequired();

		// Text
		builder.Property(ts => ts.Text)
			.IsRequired();

		// Type
		builder.Property(v => v.Type)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// StartTime
		builder.Property(ts => ts.StartTime)
			.IsRequired();

		// EndTime
		builder.Property(ts => ts.EndTime)
			.IsRequired();

		// Confidence
		builder.Property(x => x.Confidence)
			.HasPrecision(5, 4)
			.IsRequired();

		// TranscriptSegmentId
		builder.Property(ts => ts.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value));
		builder.HasOne(ts => ts.TranscriptSegment)
			.WithMany(t => t.TranscriptItems)
			.HasForeignKey(ts => ts.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
