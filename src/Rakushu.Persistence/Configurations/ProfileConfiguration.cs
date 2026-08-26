using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Configurations;

public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
	public void Configure(EntityTypeBuilder<Profile> builder)
	{
		builder.ToTable("profiles");

		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id)
			.HasColumnName("user_id");

		builder.Property(p => p.DisplayName)
			.HasColumnName("display_name")
			.HasMaxLength(100);

		builder.Property(p => p.AvatarUrl)
			.HasColumnName("avatar_url")
			.HasMaxLength(500);

		builder.Property(p => p.Bio)
			.HasColumnName("bio");

		builder.Property(p => p.NativeLanguage)
			.HasColumnName("native_language")
			.HasMaxLength(50);

		builder.Property(p => p.LearningLanguage)
			.HasColumnName("learning_language")
			.HasMaxLength(50);

		builder.Property(p => p.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(p => p.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();
	}
}
