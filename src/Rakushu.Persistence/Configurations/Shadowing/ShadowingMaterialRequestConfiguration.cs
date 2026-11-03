using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingMaterialRequestConfiguration : IEntityTypeConfiguration<ShadowingMaterialRequest>
{
	public void Configure(EntityTypeBuilder<ShadowingMaterialRequest> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialRequestId.From(value));

		// Code
		builder.Property(m => m.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(m => m.Code)
			.IsUnique();

		// RejectReason
		builder.Property(m => m.RejectReason);

		// Status (enum)
		builder.Property(s => s.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// VideoId
		builder.Property(m => m.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne<Video>(x => x.Video)
			.WithOne(v => v.ShadowingMaterialRequest)
			.HasForeignKey<ShadowingMaterialRequest>(m => m.VideoId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(m => m.VideoId)
			.IsUnique();

		// LearnerId
		builder.Property(m => m.LearnerId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne<User>(x => x.Learner)
			.WithMany(v => v.ShadowingMaterialRequests)
			.HasForeignKey(m => m.LearnerId)
			.OnDelete(DeleteBehavior.Restrict);

		// RequestedAt
		builder.Property(m => m.RequestedAt).IsRequired();

		// UpdatedAt
		builder.Property(m => m.UpdatedAt).IsRequired();

		// CompletedAt
		builder.Property(m => m.CompletedAt);

	}
}
