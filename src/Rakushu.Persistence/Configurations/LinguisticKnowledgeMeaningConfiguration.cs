using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LinguisticKnowledge;
using Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgeMeaning;

namespace Rakushu.Persistence.Configurations;

internal sealed class LinguisticKnowledgeMeaningConfiguration : IEntityTypeConfiguration<LinguisticKnowledgeMeaning>
{
	public void Configure(EntityTypeBuilder<LinguisticKnowledgeMeaning> builder)
	{
		// Id
		builder.HasKey(km => km.Id);
		builder.Property(km => km.Id)
			.HasConversion(
				id => id.Value,
				value => LinguisticKnowledgeMeaningId.From(value));

		// LinguisticKnowledgeId
		builder.Property(km => km.LinguisticKnowledgeId)
			.HasConversion(
				id => id.Value,
				value => LinguisticKnowledgeId.From(value))
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
