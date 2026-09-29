using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.ContentCategory.ContentProcessingPolicy;

namespace Rakushu.Persistence.Configurations;

internal sealed class ContentProcessingPolicyConfiguration : IEntityTypeConfiguration<ContentProcessingPolicy>
{
	public void Configure(EntityTypeBuilder<ContentProcessingPolicy> builder)
	{
		// Id
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.HasConversion(
				id => id.Value,
				value => ContentProcessingPolicyId.From(value));

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
			.HasMaxLength(50)
			.IsRequired();

		// Name
		builder.Property(c => c.Name)
			.HasMaxLength(200)
			.IsRequired();

		// Description
		builder.Property(c => c.Description)
			.HasMaxLength(1000);

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
