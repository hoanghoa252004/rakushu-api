using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
	public void Configure(EntityTypeBuilder<Feature> builder)
	{
		// Id
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id)
			.HasConversion(
				id => id.Value,
				value => FeatureId.From(value));


	}
}
