using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.Profile;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;

namespace Rakushu.Persistence.Configurations.Identity;

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
	public void Configure(EntityTypeBuilder<Profile> builder)
	{
		// Schema
		builder.Metadata.SetSchema("identity");

		// Id
		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id)
			.HasConversion(
				id => id.Value,
				value => ProfileId.From(value));

		// UserId
		builder.Property(p => p.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(p => p.User)
			.WithOne(u => u.Profile)
			.HasForeignKey<Profile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(p => p.UserId)
			.IsUnique();

		// AvatarKey
		builder.Property(p => p.AvatarKey)
			.HasMaxLength(100);

		// Level
		builder.Property(p => p.LevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(p => p.Level)
			.WithMany(cl => cl.ProfileLevels)
			.HasForeignKey(p => p.LevelId)
			.OnDelete(DeleteBehavior.Restrict);

		// DailyLearningMinutes
		builder.Property(p => p.DailyLearningMinutes)
			.IsRequired();

		// SessionDurationMinutes
		builder.Property(p => p.SessionDurationMinutes)
			.IsRequired();

		// CreatedAt
		builder.Property(p => p.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(p => p.UpdatedAt)
			.IsRequired();
	}
}
