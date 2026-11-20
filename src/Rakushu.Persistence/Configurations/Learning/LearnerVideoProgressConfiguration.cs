using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Learning;

internal sealed class LearnerVideoProgressConfiguration : IEntityTypeConfiguration<LearnerVideoProgress>
{
	public void Configure(EntityTypeBuilder<LearnerVideoProgress> builder)
	{
		builder.ToTable("learner_video_progresses", "learning");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasConversion(
				id => id.Value,
				value => LearnerVideoProgressId.From(value));

		builder.Property(x => x.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.Property(x => x.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();

		builder.Property(x => x.LastWatchedSecond)
			.IsRequired()
			.HasDefaultValue(0d);

		builder.Property(x => x.TotalWatchedSeconds)
			.IsRequired()
			.HasDefaultValue(0d);

		builder.Property(x => x.WatchedIntervalsJson)
			.HasColumnType("jsonb")
			.IsRequired()
			.HasDefaultValueSql("'[]'::jsonb");

		builder.Property(x => x.CompletionRate)
			.IsRequired()
			.HasDefaultValue(0d);

		builder.Property(x => x.IsCompleted)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.LastWatchedAt)
			.IsRequired();

		builder.HasOne(x => x.User)
			.WithMany()
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.Video)
			.WithMany()
			.HasForeignKey(x => x.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(x => new { x.UserId, x.VideoId })
			.IsUnique();

		builder.HasIndex(x => new { x.UserId, x.LastWatchedAt });
	}
}
