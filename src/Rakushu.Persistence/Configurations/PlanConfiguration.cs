using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Plan;
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

		// Code
		builder.Property(pl => pl.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name
		builder.Property(u => u.Name)
			.HasMaxLength(100)
			.IsRequired();

		// JapaneseName
		builder.Property(u => u.JapaneseName)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(u => u.Description);

		// Price
		builder.Property(u => u.Price)
			.HasPrecision(10,2)
			.IsRequired();

		// Currency
		builder.Property(u => u.Currency)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// BillingCycle
		builder.Property(u => u.BillingCycle)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// IsActive
		builder.Property(u => u.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}

