using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingAttemptDetailConfiguration : IEntityTypeConfiguration<ShadowingAttemptDetail>
{
	public void Configure(EntityTypeBuilder<ShadowingAttemptDetail> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingAttemptDetailId.From(value));

		// ShadowingAttemptId
		builder.Property(d => d.ShadowingAttemptId)
			.HasConversion(
				id => id.Value,
				value => ShadowingAttemptId.From(value))
			.IsRequired();
		builder.HasOne(d => d.ShadowingAttempt)
			.WithMany(a => a.Details)
			.HasForeignKey(d => d.ShadowingAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		// ShadowingRoleLineId
		builder.Property(d => d.ShadowingRoleLineId)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleLineId.From(value))
			.IsRequired();
		builder.HasOne(d => d.ShadowingRoleLine)
			.WithMany(x => x.AttemptDetails)
			.HasForeignKey(d => d.ShadowingRoleLineId)
			.OnDelete(DeleteBehavior.Restrict);

		// One detail per role line in an attempt
		builder.HasIndex(d => new { d.ShadowingAttemptId, d.ShadowingRoleLineId })
			.IsUnique();

		// AudioKey
		builder.Property(d => d.AudioKey)
			.HasMaxLength(500);

		// DurationMs
		builder.Property(d => d.DurationMs);

		// IsSkipped
		builder.Property(d => d.IsSkipped)
			.IsRequired()
			.HasDefaultValue(false);

		// SubmittedAt
		builder.Property(d => d.SubmittedAt);
	}
}
