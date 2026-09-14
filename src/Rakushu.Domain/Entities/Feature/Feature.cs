using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Feature.ObjectValues;
using Rakushu.Domain.Entities.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature;

public sealed class Feature : AggregateRoot<FeatureId>
{
	public FeatureCode Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string Description { get; private set; } = null!;
	public FeatureStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
}
