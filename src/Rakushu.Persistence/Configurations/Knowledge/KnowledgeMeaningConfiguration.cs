using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeMeaningConfiguration : IEntityTypeConfiguration<KnowledgeMeaning>
{
	public void Configure(EntityTypeBuilder<KnowledgeMeaning> builder)
	{
		// Id
		builder.HasKey(km => km.Id);
		builder.Property(km => km.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value));

		// LinguisticKnowledgeId
		builder.Property(km => km.LinguisticKnowledgeId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value))
			.IsRequired();
		builder.HasOne(km => km.LinguisticKnowledge)
			.WithMany(lk => lk.Meanings)
			.HasForeignKey(km => km.LinguisticKnowledgeId)
			.OnDelete(DeleteBehavior.Cascade);

		// SortOrder
		builder.Property(km => km.SortOrder)
			.IsRequired();

		// Meaning
		builder.Property(km => km.Meaning)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(km => km.Description);

		// CreatedAt
		builder.Property(km => km.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(km => km.UpdatedAt)
			.IsRequired();
	}
}
