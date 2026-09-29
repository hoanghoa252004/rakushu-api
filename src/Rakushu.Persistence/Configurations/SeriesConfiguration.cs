using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;

namespace Rakushu.Persistence.Configurations;

internal sealed class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
	public void Configure(EntityTypeBuilder<Series> builder)
	{
		// Id
		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => SeriesId.From(value));

		// Slug
		builder.Property(s => s.Slug)
			.HasMaxLength(200)
			.IsRequired();
		builder.HasIndex(s => s.Slug)
			.IsUnique();

		// Name
		builder.Property(s => s.Name)
			.HasMaxLength(255)
			.IsRequired();

		// Description
		builder.Property(s => s.Description)
			.HasMaxLength(1000);

		// DisplayOrder
		builder.Property(s => s.DisplayOrder)
			.IsRequired();

		// SourceType
		builder.Property(s => s.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// ContentCategoryId
		builder.Property(s => s.ContentCategoryId)
			.HasConversion(
				id => id.Value,
				value => ContentCategoryId.From(value))
			.IsRequired();
		builder.HasOne(s => s.ContentCategory)
			.WithMany(ct => ct.Series)
			.HasForeignKey(s => s.ContentCategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		// IsActive
		builder.Property(s => s.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(s => s.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(s => s.UpdatedAt)
			.IsRequired();
	}
}
