using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Subscription;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;


internal sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
	public void Configure(EntityTypeBuilder<Plan> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => PlanId.From(value));

		// Email
		builder.Property(u => u.Email)
			.HasConversion(
				email => email.Value,
				value => Email.Create(value).Value)
			.HasMaxLength(256)
			.IsRequired();
		builder.HasIndex(u => u.Email)
			.IsUnique();

		// PasswordHash
		builder.Property(u => u.PasswordHash)
			.IsRequired();

		// [VO] Profile
		builder.ComplexProperty(
			u => u.Profile,
			profile =>
			{
				// FullName
				profile.Property(p => p.FullName)
					.HasMaxLength(50)
					.IsRequired();

				// AvatarKey
				profile.Property(p => p.AvatarKey)
					.HasMaxLength(100);

				// NativeLanguage
				profile.Property(p => p.NativeLanguage)
					.HasMaxLength(50)
					.IsRequired();
			});

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

