using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;

namespace Rakushu.Persistence.Configurations;

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
				value => LinguisticKnowledgeId.From(value))
			.IsRequired();
		builder.HasOne(km => km.LinguisticKnowledge)
			.WithMany(lk => lk.Meanings)
			.HasForeignKey(km => km.LinguisticKnowledgeId)
			.OnDelete(DeleteBehavior.Cascade);

		// SortOrder
		builder.Property(km => km.SortOrder)
			.IsRequired();

		// UniversalMeaning
		builder.Property(km => km.UniversalMeaning)
			.HasMaxLength(500)
			.IsRequired();

		// CreatedAt
		builder.Property(km => km.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(km => km.UpdatedAt)
			.IsRequired();
	}
}
