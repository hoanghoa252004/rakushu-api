using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingAttemptConfiguration : IEntityTypeConfiguration<ShadowingAttempt>
{
	public void Configure(EntityTypeBuilder<ShadowingAttempt> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingAttemptId.From(value));

		// ShadowingSessionId
		builder.Property(a => a.ShadowingSessionId)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionId.From(value))
			.IsRequired();
		builder.HasOne(a => a.ShadowingSession)
			.WithMany(s => s.Attempts)
			.HasForeignKey(a => a.ShadowingSessionId)
			.OnDelete(DeleteBehavior.Cascade);

		// ShadowingSessionMemberId
		builder.Property(a => a.ShadowingSessionMemberId)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionMemberId.From(value))
			.IsRequired();
		builder.HasOne(a => a.ShadowingSessionMember)
			.WithMany(m => m.Attempts)
			.HasForeignKey(a => a.ShadowingSessionMemberId)
			.OnDelete(DeleteBehavior.Restrict);

		// Status (enum)
		builder.Property(a => a.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// StartedAt
		builder.Property(a => a.StartedAt)
			.IsRequired();

		// CompletedAt
		builder.Property(a => a.CompletedAt);
	}
}
