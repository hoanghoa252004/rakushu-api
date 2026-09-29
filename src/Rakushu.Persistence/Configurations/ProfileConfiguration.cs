using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.SupportedLanguage;

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

		// FullName
		builder.Property(p => p.FullName)
			.HasMaxLength(50)
			.IsRequired();

		// AvatarKey
		builder.Property(p => p.AvatarKey)
			.HasMaxLength(100);

		// NativeLanguageId
		builder.Property(p => p.NativeLanguageId)
			.HasConversion(
				id => id.Value,
				value => SupportedLanguageId.From(value))
			.IsRequired();
		builder.HasOne(p => p.NativeLanguage)
			.WithMany(nt => nt.Profiles)
			.HasForeignKey(p => p.NativeLanguageId)
			.OnDelete(DeleteBehavior.Restrict);

		// CurrentLevelId
		builder.Property(p => p.CurrentLevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(p => p.CurrentLevel)
			.WithMany(cl => cl.CurrentLevelProfiles)
			.HasForeignKey(p => p.CurrentLevelId)
			.OnDelete(DeleteBehavior.Restrict);

		// TargetLevelId
		builder.Property(p => p.TargetLevelId)
			.HasConversion(
				id => id.Value,
				value => ProficiencyLevelId.From(value))
			.IsRequired();
		builder.HasOne(p => p.TargetLevel)
			.WithMany(tl => tl.TargetLevelProfiles)
			.HasForeignKey(p => p.TargetLevelId)
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
