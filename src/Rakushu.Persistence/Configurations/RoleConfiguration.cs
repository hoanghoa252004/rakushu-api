using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		// Id
		builder.HasKey(r => r.Id);
		builder.Property(r => r.Id)
			.HasConversion(
				id => id.Value,
				value => RoleId.From(value));

		// Code
		builder.Property(r => r.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(r => r.Code)
			.IsUnique();

		// Name
		builder.Property(r => r.Name)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(r => r.Description);

		// CreatedAt
		builder.Property(r => r.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(r => r.UpdatedAt)
			.IsRequired();
	}
}
