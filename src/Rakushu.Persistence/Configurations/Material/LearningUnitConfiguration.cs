using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class LearningUnitConfiguration : IEntityTypeConfiguration<LearningUnit>
{
	public void Configure(EntityTypeBuilder<LearningUnit> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(l => l.Id);
		builder.Property(l => l.Id)
			.HasConversion(
				id => id.Value,
				value => LearningUnitId.From(value));

		// KnowledgeMeaningId
		builder.Property(l => l.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();
		builder.HasOne(l => l.KnowledgeMeaning)
			.WithMany(xx => xx.LearningUnits)
			.HasForeignKey(l => l.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Cascade);

		// TranscriptSegmentId
		builder.Property(l => l.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(l => l.TranscriptSegment)
			.WithMany(xx => xx.LearningUnits)
			.HasForeignKey(l => l.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Cascade);

		// StartTokenIndex
		builder.Property(l => l.StartTokenIndex)
			.IsRequired();

		// EndTokenIndex
		builder.Property(l => l.EndTokenIndex)
			.IsRequired();

		// Confidence
		builder.Property(l => l.Confidence)
			.IsRequired();

		// IsReviewed
		builder.Property(l => l.IsReviewed)
			.HasDefaultValue(false)
			.IsRequired();

		// DetectionMethod
		builder.Property(l => l.DetectionMethod)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();
	}
}
