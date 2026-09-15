using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class PlanEntitlementConfiguration : IEntityTypeConfiguration<PlanEntitlement>
{
	public void Configure(EntityTypeBuilder<PlanEntitlement> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => PlanEntitlementId.From(value));

		// PlanId
		builder.Property(u => u.PlanId)
			.HasConversion(
				id => id.Value,
				value => PlanId.From(value))
			.IsRequired();
		builder.HasOne(pe => pe.Plan)
			.WithMany(p => p.PlanEntitlements)
			.HasForeignKey(pe => pe.PlanId)
			.OnDelete(DeleteBehavior.Cascade);

		// FeatureId
		builder.Property(u => u.FeatureId)
			.HasConversion(
				id => id.Value,
				value => FeatureId.From(value))
			.IsRequired();
		builder.HasOne(pe => pe.Feature)
			.WithMany(f => f.PlanEntitlements)
			.HasForeignKey(pe => pe.FeatureId)
			.OnDelete(DeleteBehavior.Restrict);

		// IsEnabled
		builder.Property(u => u.IsEnabled)
			.HasDefaultValue(false)
			.IsRequired();

		// LimitValue
		builder.Property(u => u.LimitValue)
			.IsRequired();

		// LimitUnit
		builder.Property(u => u.LimitUnit)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// LimitPeriod
		builder.Property(u => u.LimitPeriod)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// *** COMPOSITION UNIQUE KEY ***
		builder.HasIndex(x => new
		{
			x.PlanId,
			x.FeatureId,
		})
		.IsUnique();
	}
}
