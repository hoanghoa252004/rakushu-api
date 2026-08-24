using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities;

namespace Rakushu.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasColumnName("user_id");

		builder.Property(u => u.RoleId)
			.HasColumnName("role_id")
			.IsRequired();

		builder.Property(u => u.Username)
			.HasColumnName("username")
			.HasMaxLength(50)
			.IsRequired();

		builder.HasIndex(u => u.Username)
			.IsUnique();

		builder.Property(u => u.Email)
			.HasColumnName("email")
			.HasMaxLength(256)
			.IsRequired();

		builder.HasIndex(u => u.Email)
			.IsUnique();

		builder.Property(u => u.PasswordHash)
			.HasColumnName("password_hash")
			.IsRequired();

		builder.Property(u => u.Status)
			.HasColumnName("status")
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(u => u.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(u => u.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();

		// Relationships
		builder.HasOne(u => u.Role)
			.WithMany(r => r.Users)
			.HasForeignKey(u => u.RoleId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(u => u.Profile)
			.WithOne(p => p.User)
			.HasForeignKey<Profile>(p => p.Id)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
