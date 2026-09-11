using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.EmailVerificationToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
	public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => EmailVerificationTokenId.From(value));

		// UserId
		builder.Property(x => x.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		// CodeHash
		builder.Property(x => x.CodeHash)
			.IsRequired();

		// CreatedAt
		builder.Property(x => x.CreatedAt)
			.IsRequired();

		// ExpiresAt
		builder.Property(x => x.ExpiresAt)
			.IsRequired();

		// UsedAt
		builder.Property(x => x.UsedAt);
	}
}
