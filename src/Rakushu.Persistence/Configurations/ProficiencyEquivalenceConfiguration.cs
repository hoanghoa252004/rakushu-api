using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Persistence.Configurations;

internal sealed class ProficiencyEquivalenceConfiguration : IEntityTypeConfiguration<ProficiencyEquivalence>
{
	public void Configure(EntityTypeBuilder<ProficiencyEquivalence> builder)
	{
		// Id
		builder.HasKey(pe => pe.Id);
		builder.Property(pe => pe.Id)
			.HasConversion(
				id => id.Value,
				value => ProficiencyEquivalenceId.From(value));

		// SourceLevelId
		builder.Property(pe => pe.SourceLevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(pe => pe.SourceLevel)
			.WithMany(pl => pl.SourceLevelProficiencyEquivalences)
			.HasForeignKey(pe => pe.SourceLevelId)
			.OnDelete(DeleteBehavior.Cascade);

		// TargetLevelId
		builder.Property(pe => pe.TargetLevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(pe => pe.TargetLevel)
			.WithMany(pl => pl.TargetLevelProficiencyEquivalences)
			.HasForeignKey(pe => pe.TargetLevelId)
			.OnDelete(DeleteBehavior.Restrict);

		// Type
		builder.Property(pe => pe.Type)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Note
		builder.Property(pe => pe.Note);

		// Reference
		builder.Property(pe => pe.Reference);

		// CreatedAt
		builder.Property(pe => pe.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(pe => pe.UpdatedAt)
			.IsRequired();
	}
}
