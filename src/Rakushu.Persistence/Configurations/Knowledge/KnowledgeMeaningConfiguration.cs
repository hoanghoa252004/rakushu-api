using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeMeaningConfiguration : IEntityTypeConfiguration<KnowledgeMeaning>
{
	public void Configure(EntityTypeBuilder<KnowledgeMeaning> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(km => km.Id);
		builder.Property(km => km.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value));

		// KnowledgeUnitId
		builder.Property(km => km.KnowledgeUnitId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value))
			.IsRequired();
		builder.HasOne(km => km.KnowledgeUnit)
			.WithMany(lk => lk.Meanings)
			.HasForeignKey(km => km.KnowledgeUnitId)
			.OnDelete(DeleteBehavior.Cascade);

		// SortOrder
		builder.Property(km => km.SortOrder)
			.IsRequired();

		// Meaning
		builder.Property(km => km.Meaning)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseMeaning
		builder.Property(km => km.JapaneseMeaning)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(km => km.Description);
	}
}
