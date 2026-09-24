using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;

namespace Rakushu.Persistence.Configurations;

internal sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
	public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
	{
		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => PaymentTransactionId.From(value));

		// PaymentId
		builder.Property(t => t.PaymentId)
			.HasConversion(
				id => id.Value,
				value => PaymentId.From(value))
			.IsRequired();
		builder.HasOne(t => t.Payment)
			.WithMany(p => p.Transactions)
			.HasForeignKey(t => t.PaymentId)
			.OnDelete(DeleteBehavior.Cascade);

		// SepayId (Unique index for Webhook idempotency, only when not null)
		builder.Property(t => t.SepayId)
			.IsRequired(false);
		builder.HasIndex(t => t.SepayId)
			.IsUnique()
			.HasFilter("sepay_id IS NOT NULL");

		// ExpiresAt
		builder.Property(t => t.ExpiresAt)
			.IsRequired();

		// Gateway
		builder.Property(t => t.Gateway)
			.HasMaxLength(50)
			.IsRequired();

		// AccountNumber
		builder.Property(t => t.AccountNumber)
			.HasMaxLength(50)
			.IsRequired();

		// TransactionDate
		builder.Property(t => t.TransactionDate)
			.IsRequired();

		// Content
		builder.Property(t => t.Content)
			.HasColumnType("text")
			.IsRequired();

		// TransferType
		builder.Property(t => t.TransferType)
			.HasMaxLength(10)
			.IsRequired();

		// TransferAmount
		builder.Property(t => t.TransferAmount)
			.HasPrecision(18, 2)
			.IsRequired();

		// ReferenceCode
		builder.Property(t => t.ReferenceCode)
			.HasMaxLength(100);

		// Status
		builder.Property(t => t.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// RawWebhookData
		builder.Property(t => t.RawWebhookData)
			.HasColumnType("text");

		// CreatedAt
		builder.Property(t => t.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(t => t.UpdatedAt)
			.IsRequired();
	}
}
