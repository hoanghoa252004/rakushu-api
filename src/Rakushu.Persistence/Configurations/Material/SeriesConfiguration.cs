using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
	public void Configure(EntityTypeBuilder<Series> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => SeriesId.From(value));

		// UserId
		builder.Property(v => v.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value));
		builder.HasOne(v => v.User)
			.WithMany(u => u.Series)
			.HasForeignKey(v => v.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		// Slug
		builder.Property(s => s.Slug)
			.HasMaxLength(200)
			.IsRequired();
		builder.HasIndex(s => s.Slug)
			.IsUnique();

		// Name
		builder.Property(s => s.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName
		builder.Property(s => s.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// ThumbnailKey
		builder.Property(m => m.ThumbnailKey)
			.HasMaxLength(500)
			.IsRequired();

		// Description
		builder.Property(s => s.Description);

		// DisplayOrder
		builder.Property(s => s.DisplayOrder)
			.IsRequired();

		// SourceType
		builder.Property(s => s.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

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
