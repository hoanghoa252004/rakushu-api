using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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


	}
}