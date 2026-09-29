using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;

namespace Rakushu.Persistence.Configurations;

internal sealed class LinguisticKnowledgeConfiguration : IEntityTypeConfiguration<LinguisticKnowledge>
{
	public void Configure(EntityTypeBuilder<LinguisticKnowledge> builder)
	{
		// Id
		builder.HasKey(lk => lk.Id);
		builder.Property(lk => lk.Id)
			.HasConversion(
				id => id.Value,
				value => LinguisticKnowledgeId.From(value));

		// KnowledgeType (enum)
		builder.Property(lk => lk.KnowledgeType)
			.HasConversion<string>()
			.IsRequired();

		// Expression
		builder.Property(lk => lk.Expression)
			.HasMaxLength(255)
			.IsRequired();

		// Reading
		builder.Property(lk => lk.Reading)
			.HasMaxLength(255)
			.IsRequired();

		// SourceType (enum)
		builder.Property(lk => lk.SourceType)
			.HasConversion<string>()
			.IsRequired();

		// Status (enum)
		builder.Property(lk => lk.Status)
			.HasConversion<string>()
			.IsRequired();

		// CreatedAt
		builder.Property(lk => lk.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(lk => lk.UpdatedAt)
			.IsRequired();
	}
}
