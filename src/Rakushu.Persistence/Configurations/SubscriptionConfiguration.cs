using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
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


	}
}