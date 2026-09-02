using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Role;

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
				value => RoleId.From(value)); ;

		// Title
		builder.Property(r => r.Title)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(r => r.Title)
			.IsUnique();

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
