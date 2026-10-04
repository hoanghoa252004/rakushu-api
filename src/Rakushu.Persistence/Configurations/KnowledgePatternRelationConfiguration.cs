using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternRelation;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Persistence.Configurations;

internal sealed class KnowledgePatternRelationConfiguration : IEntityTypeConfiguration<KnowledgePatternRelation>
{
	public void Configure(EntityTypeBuilder<KnowledgePatternRelation> builder)
	{
		// Id
		builder.HasKey(kpr => kpr.Id);
		builder.Property(kpr => kpr.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternRelationId.From(value));

		// KnowledgePatternId (FK to KnowledgePattern)
		builder.Property(kpr => kpr.KnowledgePatternId)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternId.From(value))
			.IsRequired();
		builder.HasOne(kpr => kpr.KnowledgePattern)
			.WithMany(kp => kp.Relations)
			.HasForeignKey(kpr => kpr.KnowledgePatternId)
			.OnDelete(DeleteBehavior.Cascade);

		// FromElementId (FK to KnowledgePatternElement - OUTGOING)
		builder.Property(kpr => kpr.FromElementId)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternElementId.From(value))
			.IsRequired();
		builder.HasOne(kpr => kpr.FromElement)
			.WithMany(kpe => kpe.OutgoingRelations)
			.HasForeignKey(kpr => kpr.FromElementId)
			.OnDelete(DeleteBehavior.Cascade);

		// ToElementId (FK to KnowledgePatternElement - INCOMING)
		builder.Property(kpr => kpr.ToElementId)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternElementId.From(value))
			.IsRequired();
		builder.HasOne(kpr => kpr.ToElement)
			.WithMany(kpe => kpe.IncomingRelations)
			.HasForeignKey(kpr => kpr.ToElementId)
			.OnDelete(DeleteBehavior.Cascade);

		// RelationType (enum)
		builder.Property(kpr => kpr.RelationType)
			.HasConversion<string>()
			.IsRequired();

		// DependencyRelationshipId (optional FK to DependencyRelationship)
		builder.Property(kpr => kpr.DependencyRelationshipId)
			.HasConversion(
				id => id == null ? (Guid?)null : id.Value,
				value => value == null ? null : DependencyRelationshipId.From(value.Value));
		builder.HasOne(kpr => kpr.DependencyRelationship)
			.WithMany(xx => xx.KnowledgePatternRelations)
			.HasForeignKey(kpr => kpr.DependencyRelationshipId)
			.OnDelete(DeleteBehavior.Restrict);

		// CreatedAt
		builder.Property(kpr => kpr.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(kpr => kpr.UpdatedAt)
			.IsRequired();
	}
}
