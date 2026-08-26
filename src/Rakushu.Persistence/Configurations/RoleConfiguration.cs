using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Role;

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
	}
}
