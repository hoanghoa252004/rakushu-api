using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Learning;

internal sealed class LearnerEncounterEventConfiguration : IEntityTypeConfiguration<LearnerEncounterEvent>
{
	public void Configure(EntityTypeBuilder<LearnerEncounterEvent> builder)
	{
		builder.ToTable("learner_encounter_events", "learning");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasConversion(
				id => id.Value,
				value => LearnerEncounterEventId.From(value));

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

		builder.Property(x => x.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();

		builder.Property(x => x.EncounteredAt)
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

		builder.HasOne(x => x.Video)
			.WithMany()
			.HasForeignKey(x => x.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(x => new { x.UserId, x.KnowledgeMeaningId, x.EncounteredAt });
		builder.HasIndex(x => new { x.UserId, x.VideoId });
		builder.HasIndex(x => x.LearningUnitId);
	}
}
