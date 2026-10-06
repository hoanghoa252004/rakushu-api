using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ProficiencyLevel;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Persistence.Configurations;

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
	public void Configure(EntityTypeBuilder<Profile> builder)
	{
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
