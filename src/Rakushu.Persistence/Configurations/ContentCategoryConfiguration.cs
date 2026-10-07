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
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(c => c.Code)
			.IsUnique();

		// Name
		builder.Property(c => c.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName
		builder.Property(c => c.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(c => c.Description);

		// DisplayOrder
		builder.Property(c => c.DisplayOrder)
			.IsRequired();

		// ThemeColor
		builder.Property(c => c.ThemeColor)
			.HasMaxLength(10)
			.IsRequired();

		// IsActive
		builder.Property(pl => pl.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(c => c.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(c => c.UpdatedAt)
			.IsRequired();
	}
}
