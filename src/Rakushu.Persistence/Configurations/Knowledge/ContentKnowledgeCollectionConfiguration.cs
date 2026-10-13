using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class ContentKnowledgeCollectionConfiguration : IEntityTypeConfiguration<ContentKnowledgeCollection>
{
	public void Configure(EntityTypeBuilder<ContentKnowledgeCollection> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.HasConversion(
				id => id.Value,
				value => ContentKnowledgeCollectionId.From(value));

		// ContentCategoryId
		builder.Property(c => c.ContentCategoryId)
			.HasConversion(
				id => id.Value,
				value => ContentCategoryId.From(value))
			.IsRequired();
		builder.HasOne(c => c.ContentCategory)
			.WithMany(cat => cat.ContentKnowledgeCollections)
			.HasForeignKey(c => c.ContentCategoryId)
			.OnDelete(DeleteBehavior.Cascade);

		// KnowledgeCollectionId
		builder.Property(c => c.KnowledgeCollectionId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeCollectionId.From(value))
			.IsRequired();
		builder.HasOne(c => c.KnowledgeCollection)
			.WithMany(cat => cat.ContentKnowledgeCollections)
			.HasForeignKey(c => c.KnowledgeCollectionId)
			.OnDelete(DeleteBehavior.Cascade);

		// Priority
		builder.Property(c => c.Priority)
			.IsRequired();

		// IsActive
		builder.Property(c => c.IsActive)
			.IsRequired();
	}
}
