using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using Rakushu.Domain.Entities.Subscription;
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


	}
}
