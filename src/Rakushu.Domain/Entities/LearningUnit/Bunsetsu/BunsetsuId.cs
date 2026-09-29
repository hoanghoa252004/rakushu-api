using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

public sealed class BunsetsuId : StronglyTypedId<Guid>
{
	private BunsetsuId(Guid value) : base(value)
	{
	}

	public static BunsetsuId Create() => new(Guid.NewGuid());

	public static BunsetsuId From(Guid value) => new(value);
}