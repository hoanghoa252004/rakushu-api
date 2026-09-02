using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;

namespace Rakushu.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => RefreshTokenId.From(value));

		// UserId
		builder.Property(t => t.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(t => t.User)
			.WithMany(u => u.RefreshTokens)
			.HasForeignKey(t => t.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// TokenHash
		builder.Property(t => t.TokenHash)
			.IsRequired();
		builder.HasIndex(t => t.TokenHash)
			.IsUnique();

		// IsRevoked
		builder.Property(t => t.IsRevoked)
			.HasDefaultValue(false)
			.IsRequired();

		// CreatedAt
		builder.Property(t => t.CreatedAt)
			.IsRequired();

		// ExpiresAt
		builder.Property(t => t.ExpiresAt)
			.IsRequired();

		// UsedAt
		builder.Property(t => t.ExpiresAt);
	}
}
