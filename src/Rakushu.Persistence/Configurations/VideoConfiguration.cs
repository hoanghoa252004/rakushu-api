using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Persistence.Configurations;

internal sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
	public void Configure(EntityTypeBuilder<Video> builder)
	{
		// Id
		builder.HasKey(v => v.Id);
		builder.Property(v => v.Id)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value));

		// Slug
		builder.Property(v => v.Slug)
			.HasMaxLength(200)
			.IsRequired();
		builder.HasIndex(v => v.Slug)
			.IsUnique();

		// Title
		builder.Property(v => v.Title)
			.HasMaxLength(255)
			.IsRequired();

		// Description
		builder.Property(v => v.Description)
			.HasMaxLength(1000);

		// Duration
		builder.Property(v => v.Duration)
			.IsRequired();

		// ContentCategoryId
		builder.Property(v => v.ContentCategoryId)
			.HasConversion(
				id => id.Value,
				value => ContentCategoryId.From(value))
			.IsRequired();
		builder.HasOne(v => v.ContentCategory)
			.WithMany(cc => cc.Videos)
			.HasForeignKey(v => v.ContentCategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		// SeriesId
		builder.Property(v => v.SeriesId)
			.HasConversion(
				id => id.Value,
				value => SeriesId.From(value))
			.IsRequired();
		builder.HasOne(v => v.Series)
			.WithMany(s => s.Videos)
			.HasForeignKey(v => v.SeriesId)
			.OnDelete(DeleteBehavior.Restrict);

		// SortOrder
		builder.Property(v => v.SortOrder)
			.IsRequired();

		// SourceType
		builder.Property(v => v.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// SourceUrl
		builder.Property(v => v.SourceUrl)
			.HasMaxLength(500);

		// Status
		builder.Property(v => v.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// CreatedBy (UserId)
		builder.Property(v => v.CreatedBy)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(v => v.CreatedByUser)
			.WithMany(u => u.Videos)
			.HasForeignKey(v => v.CreatedBy)
			.OnDelete(DeleteBehavior.Restrict);

		// CreatedAt
		builder.Property(v => v.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(v => v.UpdatedAt)
			.IsRequired();
	}
}
