using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingSessionInvitationConfiguration : IEntityTypeConfiguration<ShadowingSessionInvitation>
{
	public void Configure(EntityTypeBuilder<ShadowingSessionInvitation> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(i => i.Id);
		builder.Property(i => i.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionInvitationId.From(value));

		// ShadowingSessionId
		builder.Property(i => i.ShadowingSessionId)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionId.From(value))
			.IsRequired();
		builder.HasOne<ShadowingSession>()
			.WithMany()
			.HasForeignKey(i => i.ShadowingSessionId)
			.OnDelete(DeleteBehavior.Cascade);

		// HostId
		builder.Property(i => i.HostId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(i => i.HostId)
			.OnDelete(DeleteBehavior.Restrict);

		// LearnerId
		builder.Property(i => i.LearnerId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(i => i.LearnerId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(i => new { i.ShadowingSessionId, i.LearnerId });

		// Status (enum)
		builder.Property(i => i.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// SentAt
		builder.Property(i => i.SentAt)
			.IsRequired();

		// ExpiredAt
		builder.Property(i => i.ExpiredAt)
			.IsRequired();

		// ResponsedAt
		builder.Property(i => i.ResponsedAt);
	}
}
