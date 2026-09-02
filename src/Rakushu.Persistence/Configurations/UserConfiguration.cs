using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => new UserId(value));

		// Email
		builder.Property(u => u.Email)
			.HasMaxLength(256)
			.IsRequired();
		builder.HasIndex(u => u.Email)
			.IsUnique();

		// PasswordHash
		builder.Property(u => u.PasswordHash)
			.IsRequired();

		// FullName
		builder.Property(u => u.FullName)
			.HasMaxLength(50)
			.IsRequired();

		// AvatarKey
		builder.Property(u => u.AvatarKey)
			.HasMaxLength(100);

		// NativeLanguage
		builder.Property(u => u.NativeLanguage)
			.HasMaxLength(50)
			.IsRequired();

		// RoleId
		builder.Property(u => u.RoleId)
			.HasConversion(
				id => id.Value,
				value => new RoleId(value))
			.IsRequired();
		builder.HasOne(u => u.Role)
			.WithMany(r => r.Users)
			.HasForeignKey(u => u.RoleId)
			.OnDelete(DeleteBehavior.Restrict);

		// Status
		builder.Property(u => u.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}
