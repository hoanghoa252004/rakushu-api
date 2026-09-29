using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Subtitle;
using Rakushu.Domain.SupportedLanguage;

namespace Rakushu.Persistence.Configurations;

internal sealed class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
{
	public void Configure(EntityTypeBuilder<Subtitle> builder)
	{
		// Id
		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => SubtitleId.From(value));

		// Code
		builder.Property(s => s.Code)
			.HasMaxLength(50)
			.IsRequired();

		// VideoId
		builder.Property(s => s.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne(s => s.Video)
			.WithMany(v => v.Subtitles)
			.HasForeignKey(s => s.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		// SupportedLanguageId
		builder.Property(s => s.SupportedLanguageId)
			.HasConversion(
				id => id.Value,
				value => SupportedLanguageId.From(value))
			.IsRequired();
		builder.HasOne(s => s.SupportedLanguage)
			.WithMany(sl => sl.Subtitles)
			.HasForeignKey(s => s.SupportedLanguageId)
			.OnDelete(DeleteBehavior.Restrict);

		// SourceType
		builder.Property(s => s.SourceType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// StorageKey
		builder.Property(s => s.StorageKey)
			.HasMaxLength(500);

		// Status
		builder.Property(s => s.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// CreatedAt
		builder.Property(s => s.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(s => s.UpdatedAt)
			.IsRequired();
	}
}
