using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;

namespace Rakushu.Persistence.Configurations;

internal sealed class KnowledgePatternConfiguration : IEntityTypeConfiguration<KnowledgePattern>
{
	public void Configure(EntityTypeBuilder<KnowledgePattern> builder)
	{
		// Id
		builder.HasKey(kp => kp.Id);
		builder.Property(kp => kp.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgePatternId.From(value));

		// LinguisticKnowledgeId (FK to LinguisticKnowledge)
		builder.Property(kp => kp.LinguisticKnowledgeId)
			.HasConversion(
				id => id.Value,
				value => LinguisticKnowledgeId.From(value))
			.IsRequired();
		builder.HasOne(kp => kp.LinguisticKnowledge)
			.WithOne(lk => lk.KnowledgePattern)
			.HasForeignKey<KnowledgePattern>(kp => kp.LinguisticKnowledgeId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasIndex(kp => kp.LinguisticKnowledgeId)
			.IsUnique();

		// CreatedAt
		builder.Property(kp => kp.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(kp => kp.UpdatedAt)
			.IsRequired();
	}
}
