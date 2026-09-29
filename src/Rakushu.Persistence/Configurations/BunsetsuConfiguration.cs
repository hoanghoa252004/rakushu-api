using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations;

internal sealed class BunsetsuConfiguration : IEntityTypeConfiguration<Bunsetsu>
{
	public void Configure(EntityTypeBuilder<Bunsetsu> builder)
	{
		// Id
		builder.HasKey(b => b.Id);
		builder.Property(b => b.Id)
			.HasConversion(
				id => id.Value,
				value => BunsetsuId.From(value));

		// Sequence
		builder.Property(b => b.Sequence)
			.IsRequired();

		// Text
		builder.Property(b => b.Text)
			.IsRequired();

		// StartIndex
		builder.Property(b => b.StartIndex)
			.IsRequired();

		// EndIndex
		builder.Property(b => b.EndIndex)
			.IsRequired();

		// TranscriptSegmentId
		builder.Property(b => b.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(b => b.TranscriptSegment)
			.WithMany(ts => ts.Bunsetsu)
			.HasForeignKey(b => b.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Cascade);

		// CreatedAt
		builder.Property(b => b.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(b => b.UpdatedAt)
			.IsRequired();
	}
}
