using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Persistence.Configurations;

internal sealed class ContentCategoryConfiguration : IEntityTypeConfiguration<ContentCategory>
{
	public void Configure(EntityTypeBuilder<ContentCategory> builder)
	{
		// Id
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.HasConversion(
				id => id.Value,
				value => ContentCategoryId.From(value));

		// Slug
		builder.Property(c => c.Slug)
			.HasMaxLength(100)
			.IsRequired();
		builder.HasIndex(c => c.Slug)
			.IsUnique();

		// Code
		builder.Property(c => c.Code)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(c => c.Code)
			.IsUnique();

		// Name
		builder.Property(c => c.Name)
			.HasMaxLength(200)
			.IsRequired();

		// Description
		builder.Property(c => c.Description)
			.HasMaxLength(1000);

		// ParentId (Self-referencing foreign key)
		builder.Property(c => c.ParentId)
			.HasConversion(
				id => (Guid?)id!.Value,
				value => value.HasValue ? ContentCategoryId.From(value.Value) : null);
		builder.HasOne(c => c.Parent)
			.WithMany(c => c.Children)
			.HasForeignKey(c => c.ParentId)
			.OnDelete(DeleteBehavior.Restrict);

		// Level
		builder.Property(c => c.Level)
			.IsRequired();

		// DisplayOrder
		builder.Property(c => c.DisplayOrder)
			.IsRequired();

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
