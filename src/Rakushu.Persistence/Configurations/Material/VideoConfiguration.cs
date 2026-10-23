using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
	public void Configure(EntityTypeBuilder<Video> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

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
				value => SeriesId.From(value));
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

		// LearnerId
		builder.Property(v => v.LearnerId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value));
		builder.HasOne(v => v.Learner)
			.WithMany(u => u.Videos)
			.HasForeignKey(v => v.LearnerId)
			.OnDelete(DeleteBehavior.Restrict);

		// CreatedAt
		builder.Property(v => v.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(v => v.UpdatedAt)
			.IsRequired();
	}
}
