using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingSessionMemberConfiguration : IEntityTypeConfiguration<ShadowingSessionMember>
{
	public void Configure(EntityTypeBuilder<ShadowingSessionMember> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionMemberId.From(value));

		// ShadowingSessionId
		builder.Property(m => m.ShadowingSessionId)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionId.From(value))
			.IsRequired();
		builder.HasOne(m => m.ShadowingSession)
			.WithMany(s => s.Members)
			.HasForeignKey(m => m.ShadowingSessionId)
			.OnDelete(DeleteBehavior.Cascade);

		// ShadowingRoleId
		builder.Property(m => m.ShadowingRoleId)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleId.From(value))
			.IsRequired();
		builder.HasOne(m => m.ShadowingRole)
			.WithMany(s => s.Members)
			.HasForeignKey(m => m.ShadowingRoleId)
			.OnDelete(DeleteBehavior.Restrict);

		// MemberId
		builder.Property(m => m.MemberId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(m => m.Member)
			.WithMany()
			.HasForeignKey(m => m.MemberId)
			.OnDelete(DeleteBehavior.Restrict);

		// One membership per user in a session
		builder.HasIndex(m => new { m.ShadowingSessionId, m.MemberId })
			.IsUnique();

		// JoinType (enum)
		builder.Property(m => m.JoinType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// ShadowingSessionInvitationId
		builder.Property(m => m.ShadowingSessionInvitationId)
			.HasConversion(
				id => id!.Value,
				value => ShadowingSessionInvitationId.From(value));
		builder.HasOne(m => m.ShadowingSessionInvitation)
			.WithMany()
			.HasForeignKey(m => m.ShadowingSessionInvitationId)
			.OnDelete(DeleteBehavior.Restrict);

		// JoinedAt
		builder.Property(m => m.JoinedAt)
			.IsRequired();
	}
}
