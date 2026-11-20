using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Learning;

internal sealed class LearnerEncounteredLearningUnitConfiguration : IEntityTypeConfiguration<LearnerEncounteredLearningUnit>
{
	public void Configure(EntityTypeBuilder<LearnerEncounteredLearningUnit> builder)
	{
		builder.ToTable("learner_encountered_learning_units", "learning");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasConversion(
				id => id.Value,
				value => LearnerEncounteredLearningUnitId.From(value));

		builder.Property(x => x.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.Property(x => x.LearningUnitId)
			.HasConversion(
				id => id.Value,
				value => LearningUnitId.From(value))
			.IsRequired();

		builder.Property(x => x.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();

		builder.Property(x => x.KnowledgeUnitId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value))
			.IsRequired();

		builder.Property(x => x.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();

		builder.Property(x => x.EncounterCount)
			.IsRequired()
			.HasDefaultValue(1);

		builder.Property(x => x.RawPlaybackCount)
			.IsRequired()
			.HasDefaultValue(1);

		builder.Property(x => x.EncounterSourcesJson)
			.HasColumnType("jsonb")
			.IsRequired()
			.HasDefaultValueSql("'[]'::jsonb");

		builder.Property(x => x.FirstEncounteredAt)
			.IsRequired();

		builder.Property(x => x.LastEncounteredAt)
			.IsRequired();

		builder.HasOne(x => x.User)
			.WithMany()
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.LearningUnit)
			.WithMany()
			.HasForeignKey(x => x.LearningUnitId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.KnowledgeMeaning)
			.WithMany()
			.HasForeignKey(x => x.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.KnowledgeUnit)
			.WithMany()
			.HasForeignKey(x => x.KnowledgeUnitId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.Video)
			.WithMany()
			.HasForeignKey(x => x.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(x => new { x.UserId, x.LearningUnitId })
			.IsUnique();

		builder.HasIndex(x => new { x.UserId, x.KnowledgeMeaningId });
		builder.HasIndex(x => new { x.UserId, x.KnowledgeUnitId });
		builder.HasIndex(x => new { x.UserId, x.VideoId });
	}
}
