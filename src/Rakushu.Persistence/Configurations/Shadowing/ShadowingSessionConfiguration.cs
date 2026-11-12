using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingSessionConfiguration : IEntityTypeConfiguration<ShadowingSession>
{
	public void Configure(EntityTypeBuilder<ShadowingSession> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(s => s.Id);
		builder.Property(s => s.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingSessionId.From(value));

		// ShadowingMaterialId
		builder.Property(s => s.ShadowingMaterialId)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialId.From(value))
			.IsRequired();
		builder.HasOne(s => s.ShadowingMaterial)
			.WithMany(m => m.Sessions)
			.HasForeignKey(s => s.ShadowingMaterialId)
			.OnDelete(DeleteBehavior.Restrict);

		// HostId
		builder.Property(s => s.HostId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(s => s.HostId)
			.OnDelete(DeleteBehavior.Restrict);

		// Title
		builder.Property(s => s.Title)
			.HasMaxLength(200)
			.IsRequired();

		// Visibility (enum)
		builder.Property(s => s.Visibility)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Status (enum)
		builder.Property(s => s.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// ScheduledStartAt
		builder.Property(s => s.ScheduledStartAt)
			.IsRequired();

		// StartedAt
		builder.Property(s => s.StartedAt);

		// EndedAt
		builder.Property(s => s.EndedAt);
	}
}
