using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class SubscriptionUsageConfiguration : IEntityTypeConfiguration<SubscriptionUsage>
{
	public void Configure(EntityTypeBuilder<SubscriptionUsage> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => SubscriptionUsageId.From(value));

		// SubscriptionId
		builder.Property(u => u.SubscriptionId)
			.HasConversion(
				id => id.Value,
				value => SubscriptionId.From(value))
			.IsRequired();
		builder.HasOne(su => su.Subscription)
			.WithMany(f => f.SubscriptionUsages)
			.HasForeignKey(pe => pe.SubscriptionId)
			.OnDelete(DeleteBehavior.Restrict);

		// FeatureId
		builder.Property(u => u.FeatureId)
			.HasConversion(
				id => id.Value,
				value => FeatureId.From(value))
			.IsRequired();
		builder.HasOne(su => su.Feature)
			.WithMany(f => f.SubscriptionUsages)
			.HasForeignKey(pe => pe.FeatureId)
			.OnDelete(DeleteBehavior.Restrict);

		// PeriodStart
		builder.Property(u => u.PeriodStart)
			.IsRequired();

		// PeriodEnd
		builder.Property(u => u.PeriodEnd)
			.IsRequired();

		// UsedValue


		// CreatedAt
		builder.Property(u => u.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(u => u.UpdatedAt)
			.IsRequired();
	}
}