using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => PaymentId.From(value));

		// UserId
		builder.Property(t => t.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(t => t.User)
			.WithMany(p => p.Payments)
			.HasForeignKey(t => t.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		// PlanId
		builder.Property(t => t.PlanId)
			.HasConversion(
				id => id.Value,
				value => PlanId.From(value))
			.IsRequired();
		builder.HasOne(t => t.Plan)
			.WithMany(p => p.Payments)
			.HasForeignKey(t => t.PlanId)
			.OnDelete(DeleteBehavior.Restrict);

		// Amount
		builder.Property(t => t.Amount)
			.HasPrecision(10, 2)
			.IsRequired();

		// Currency
		builder.Property(t => t.Currency)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Status
		builder.Property(t => t.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// ExpiredAt
		builder.Property(t => t.ExpiredAt)
			.IsRequired();

		// CreatedAt
		builder.Property(t => t.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(t => t.UpdatedAt)
			.IsRequired();
	}
}

