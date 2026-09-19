using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
	public void Configure(EntityTypeBuilder<Subscription> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => SubscriptionId.From(value));

		// UserId
		builder.Property(u => u.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(s => s.User)
			.WithMany(u => u.Subscriptions)
			.HasForeignKey(s => s.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		// PlanId
		builder.Property(u => u.PlanId)
			.HasConversion(
				id => id.Value,
				value => PlanId.From(value))
			.IsRequired();
		builder.HasOne(pe => pe.Plan)
			.WithMany(p => p.Subscriptions)
			.HasForeignKey(pe => pe.PlanId)
			.OnDelete(DeleteBehavior.Restrict);

		// StartDate
		builder.Property(u => u.StartDate)
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