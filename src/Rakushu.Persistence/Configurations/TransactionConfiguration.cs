using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
	public void Configure(EntityTypeBuilder<Transaction> builder)
	{
		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => TransactionId.From(value));

		// PaymentId
		builder.Property(t => t.PaymentId)
			.HasConversion(
				id => id.Value,
				value => PaymentId.From(value))
			.IsRequired();
		builder.HasOne(t => t.Payment)
			.WithMany(p => p.Transactions)
			.HasForeignKey(t => t.PaymentId)
			.OnDelete(DeleteBehavior.Restrict);

		// Provider
		builder.Property(t => t.Provider)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// Amount
		builder.Property(t => t.Amount)
			.HasPrecision(10, 2)
			.IsRequired();

		// Currency
		builder.Property(t => t.Currency)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// TxnRef
		builder.Property(t => t.TxnRef)
			.IsRequired();
		builder.HasIndex(t => t.TxnRef)
			.IsUnique();

		// Url
		builder.Property(t => t.Url)
			.IsRequired();
		builder.HasIndex(t => t.Url)
			.IsUnique();

		// TransactionNo
		builder.Property(t => t.TransactionNo);

		// RawResponsePayload
		builder.Property(t => t.RawResponsePayload);		

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

