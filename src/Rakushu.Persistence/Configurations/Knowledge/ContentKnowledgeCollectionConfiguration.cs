using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class ContentKnowledgeCollectionConfiguration : IEntityTypeConfiguration<ContentKnowledgeCollection>
{
	public void Configure(EntityTypeBuilder<ContentKnowledgeCollection> builder)
	{
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
			.WithMany(cat => cat.ContentProcessingPolicies)
			.HasForeignKey(c => c.ContentCategoryId)
			.OnDelete(DeleteBehavior.Cascade);

		// Code
		builder.Property(c => c.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(c => c.Code)
			.IsUnique();

		// Name
		builder.Property(c => c.Name)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(c => c.Description);

		// IsActive
		builder.Property(c => c.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(c => c.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(c => c.UpdatedAt)
			.IsRequired();
	}
}
