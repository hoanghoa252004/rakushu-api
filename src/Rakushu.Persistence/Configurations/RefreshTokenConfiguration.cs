using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities;

namespace Rakushu.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens");

		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasColumnName("token_id");

		builder.Property(t => t.UserId)
			.HasColumnName("user_id")
			.IsRequired();

		builder.Property(t => t.Token)
			.HasColumnName("token")
			.HasMaxLength(500)
			.IsRequired();

		builder.HasIndex(t => t.Token);

		builder.Property(t => t.ExpiresAt)
			.HasColumnName("expires_at")
			.IsRequired();

		builder.Property(t => t.IsRevoked)
			.HasColumnName("is_revoked")
			.IsRequired();

		builder.Property(t => t.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.HasOne(t => t.User)
			.WithMany()
			.HasForeignKey(t => t.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
