using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Enums;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Data;

public static class DbInitializer
{
	public static async Task InitializeAsync(RakushuDbContext context)
	{
		// Ensure database schema is created/migrated
		await context.Database.EnsureCreatedAsync();

		// Seed Default Roles if missing
		if (!await context.Roles.AnyAsync(r => r.Id == RoleConstants.AdminRoleId))
		{
			context.Roles.Add(new Role(
				RoleConstants.AdminRoleId,
				RoleConstants.Admin,
				"System Administrator responsible for security monitoring, user management, and business operations",
				DateTimeOffset.UtcNow));
		}

		if (!await context.Roles.AnyAsync(r => r.Id == RoleConstants.LinguisticCuratorRoleId))
		{
			context.Roles.Add(new Role(
				RoleConstants.LinguisticCuratorRoleId,
				RoleConstants.LinguisticCurator,
				"Linguistic Curator managing dictionary datasets, moderating OOV/slang terms, and monitoring AI transcription quality",
				DateTimeOffset.UtcNow));
		}

		if (!await context.Roles.AnyAsync(r => r.Id == RoleConstants.LearnerRoleId))
		{
			context.Roles.Add(new Role(
				RoleConstants.LearnerRoleId,
				RoleConstants.Learner,
				"Learner end-user with interactive video subtitles, SRS spaced repetition, and personal study space",
				DateTimeOffset.UtcNow));
		}

		await context.SaveChangesAsync();

		// Seed Default Super Admin if no admin exists
		var adminEmail = "admin@rakushu.com";
		if (!await context.Users.AnyAsync(u => u.Email.ToLower() == adminEmail.ToLower()))
		{
			var adminId = Guid.NewGuid();
			var adminPasswordHash = "$2a$11$qDmr.Xz8x06eZ6v6qY.d3OGe5i6uAwhhRsmvU.Jq2rRfZyv471r6S";
			
			var adminUser = new User(
				adminId,
				RoleConstants.AdminRoleId,
				"superadmin",
				adminEmail,
				adminPasswordHash,
				UserStatus.Active.ToString(),
				DateTimeOffset.UtcNow,
				DateTimeOffset.UtcNow);

			var adminProfile = new Profile(
				adminId,
				"System Administrator",
				null,
				"System default administrator account",
				"Vietnamese",
				"Japanese",
				DateTimeOffset.UtcNow,
				DateTimeOffset.UtcNow);

			context.Users.Add(adminUser);
			context.Profiles.Add(adminProfile);

			await context.SaveChangesAsync();
		}

		// Seed Default Linguistic Curator if missing
		var curatorEmail = "curator@rakushu.com";
		if (!await context.Users.AnyAsync(u => u.Email.ToLower() == curatorEmail.ToLower()))
		{
			var curatorId = Guid.NewGuid();
			var curatorPasswordHash = "$2a$11$qDmr.Xz8x06eZ6v6qY.d3OGe5i6uAwhhRsmvU.Jq2rRfZyv471r6S";

			var curatorUser = new User(
				curatorId,
				RoleConstants.LinguisticCuratorRoleId,
				"linguistic_curator",
				curatorEmail,
				curatorPasswordHash,
				UserStatus.Active.ToString(),
				DateTimeOffset.UtcNow,
				DateTimeOffset.UtcNow);

			var curatorProfile = new Profile(
				curatorId,
				"Linguistic Curator",
				null,
				"Platform dictionary moderator & linguistic knowledge specialist",
				"Japanese",
				"Vietnamese",
				DateTimeOffset.UtcNow,
				DateTimeOffset.UtcNow);

			context.Users.Add(curatorUser);
			context.Profiles.Add(curatorProfile);

			await context.SaveChangesAsync();
		}
	}
}
