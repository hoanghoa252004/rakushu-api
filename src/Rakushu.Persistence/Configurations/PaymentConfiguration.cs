using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		// Id
		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id)
			.HasConversion(
				id => id.Value,
				value => PaymentId.From(value));

		// UserId
		builder.Property(p => p.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(p => p.User)
			.WithMany(u => u.Payments)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		// PlanId
		builder.Property(p => p.PlanId)
			.HasConversion(
				id => id.Value,
				value => PlanId.From(value))
			.IsRequired();
		builder.HasOne(p => p.Plan)
			.WithMany(pl => pl.Payments)
			.HasForeignKey(p => p.PlanId)
			.OnDelete(DeleteBehavior.Restrict);

		// SubscriptionId (nullable)
		builder.Property(p => p.SubscriptionId)
			.HasConversion(
				id => id != null ? id.Value : (Guid?)null,
				value => value.HasValue ? SubscriptionId.From(value.Value) : null);
		builder.HasOne(p => p.Subscription)
			.WithMany(s => s.Payments)
			.HasForeignKey(p => p.SubscriptionId)
			.OnDelete(DeleteBehavior.SetNull);

		// OrderCode
		builder.Property(p => p.OrderCode)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(p => p.OrderCode)
			.IsUnique();

		// Amount
		builder.Property(p => p.Amount)
			.HasPrecision(18, 2)
			.IsRequired();

		// Currency
		builder.Property(p => p.Currency)
			.HasMaxLength(10)
			.IsRequired();

		// Status
		builder.Property(p => p.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Description
		builder.Property(p => p.Description)
			.HasMaxLength(255);

		// QrCodeUrl
		builder.Property(p => p.QrCodeUrl)
			.HasColumnType("text");

		// ExpiresAt
		builder.Property(p => p.ExpiresAt)
			.IsRequired();

		// CompletedAt
		builder.Property(p => p.CompletedAt);

		// CreatedAt
		builder.Property(p => p.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(p => p.UpdatedAt)
			.IsRequired();

		// Transactions (1 - N)
		builder.HasMany(p => p.Transactions)
			.WithOne(t => t.Payment)
			.HasForeignKey(t => t.PaymentId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
