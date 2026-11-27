using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

namespace Rakushu.Persistence.Configurations.Identity;

internal sealed class PasswordResetCodeConfiguration : IEntityTypeConfiguration<PasswordResetCode>
{
	public void Configure(EntityTypeBuilder<PasswordResetCode> builder)
	{
		// Schema
		builder.Metadata.SetSchema("identity");

		// Id
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.HasConversion(
				id => id.Value,
				value => PasswordResetCodeId.From(value));

		// UserId
		builder.Property(x => x.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// CodeHash
		builder.Property(x => x.CodeHash)
			.IsRequired();

		// CreatedAt
		builder.Property(x => x.CreatedAt)
			.IsRequired();

		// ExpiresAt
		builder.Property(x => x.ExpiresAt)
			.IsRequired();

		// IsExpired
		builder.Property(x => x.IsExpired)
			.IsRequired();

		// IsUsed
		builder.Property(x => x.IsUsed)
			.IsRequired();

		// UsedAt
		builder.Property(x => x.UsedAt);

		builder.HasIndex(x => new { x.UserId, x.CodeHash });
	}
}
