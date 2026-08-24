using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;

namespace Rakushu.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("roles");

		builder.HasKey(r => r.Id);
		builder.Property(r => r.Id)
			.HasColumnName("role_id");

		builder.Property(r => r.RoleName)
			.HasColumnName("role_name")
			.HasMaxLength(50)
			.IsRequired();

		builder.HasIndex(r => r.RoleName)
			.IsUnique();

		builder.Property(r => r.Description)
			.HasColumnName("description");

		builder.Property(r => r.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		// Seed initial roles based on project personas
		builder.HasData(
			new Role(
				RoleConstants.AdminRoleId,
				RoleConstants.Admin,
				"System Administrator responsible for security monitoring, user management, and business operations",
				DateTimeOffset.UnixEpoch),
			new Role(
				RoleConstants.LinguisticCuratorRoleId,
				RoleConstants.LinguisticCurator,
				"Linguistic Curator managing dictionary datasets, moderating OOV/slang terms, and monitoring AI transcription quality",
				DateTimeOffset.UnixEpoch),
			new Role(
				RoleConstants.LearnerRoleId,
				RoleConstants.Learner,
				"Learner end-user with interactive video subtitles, SRS spaced repetition, and personal study space",
				DateTimeOffset.UnixEpoch)
		);
	}
}
