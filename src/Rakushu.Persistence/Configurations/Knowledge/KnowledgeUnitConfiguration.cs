using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeUnitConfiguration : IEntityTypeConfiguration<KnowledgeUnit>
{
	public void Configure(EntityTypeBuilder<KnowledgeUnit> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(lk => lk.Id);
		builder.Property(lk => lk.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value));

		// KnowledgeCollectionId
		builder.Property(c => c.KnowledgeCollectionId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeCollectionId.From(value))
			.IsRequired();
		builder.HasOne(c => c.KnowledgeCollection)
			.WithMany(cat => cat.KnowledgeUnits)
			.HasForeignKey(c => c.KnowledgeCollectionId)
			.OnDelete(DeleteBehavior.Cascade);

		// ProficiencyLevelId
		builder.Property(c => c.ProficiencyLevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(c => c.ProficiencyLevel)
			.WithMany(cat => cat.KnowledgeUnits)
			.HasForeignKey(c => c.ProficiencyLevelId)
			.OnDelete(DeleteBehavior.Restrict);


		// KnowledgeUnitType (enum)
		builder.Property(lk => lk.KnowledgeType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Expression
		builder.Property(lk => lk.Expression)
			.HasMaxLength(100)
			.IsRequired();

		// Reading
		builder.Property(lk => lk.Reading)
			.HasMaxLength(100)
			.IsRequired();

		// AudioKey
		builder.Property(p => p.AudioKey)
			.HasMaxLength(100);

		// SourceType (enum)
		builder.Property(lk => lk.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// IsActive
		builder.Property(lk => lk.IsActive)
			.HasDefaultValue(false)
			.IsRequired();

		// CreatedAt
		builder.Property(lk => lk.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(lk => lk.UpdatedAt)
			.IsRequired();

		// KnowledgeHash
		builder.Property(t => t.KnowledgeHash);
	}
}
