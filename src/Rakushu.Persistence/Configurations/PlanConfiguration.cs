using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.ObjectValues;
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

		// Code
		builder.Property(u => u.Code)
			.HasConversion(
				code => code.Value,
				value => PlanCode.Create(value).Value)
			.HasMaxLength(50)
			.IsRequired();
		builder.HasIndex(u => u.Code)
			.IsUnique();

		// Name
		builder.Property(u => u.Name)
			.IsRequired();

		// Description
		builder.Property(u => u.Description);

		// Price
		builder.Property(u => u.Price)
			.HasPrecision(10,2)
			.IsRequired();

		// Currency
		builder.Property(u => u.Currency)
			.IsRequired();

		// BillingCycle
		builder.Property(u => u.BillingCycle)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

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

