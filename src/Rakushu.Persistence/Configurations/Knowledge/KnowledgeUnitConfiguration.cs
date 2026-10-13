using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeUnitConfiguration : IEntityTypeConfiguration<KnowledgeUnit>
{
	public void Configure(EntityTypeBuilder<KnowledgeUnit> builder)
	{
		// Id
		builder.HasKey(lk => lk.Id);
		builder.Property(lk => lk.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value));

		// KnowledgeType (enum)
		builder.Property(lk => lk.KnowledgeType)
			.HasMaxLength(30)
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
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Status (enum)
		builder.Property(lk => lk.Status)
			.HasMaxLength(30)
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
