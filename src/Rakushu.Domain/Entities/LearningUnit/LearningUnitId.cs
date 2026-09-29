using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit;

public sealed class LearningUnitId : StronglyTypedId<Guid>
{
	private LearningUnitId(Guid value) : base(value)
	{
	}

	public static LearningUnitId Create() => new(Guid.NewGuid());

	public static LearningUnitId From(Guid value) => new(value);
}
