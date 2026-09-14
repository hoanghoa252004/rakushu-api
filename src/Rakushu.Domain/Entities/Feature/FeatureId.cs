using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature;

public sealed class FeatureId : StronglyTypedId<Guid>
{
	private FeatureId(Guid value) : base(value)
	{
	}

	public static FeatureId Create() => new(Guid.NewGuid());

	public static FeatureId From(Guid value) => new(value);
}