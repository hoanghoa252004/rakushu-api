using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ProficiencyLevel;

public sealed class ProficiencyLevelId : StronglyTypedId<Guid>
{
	private ProficiencyLevelId(Guid value) : base(value)
	{
	}

	public static ProficiencyLevelId Create() => new(Guid.NewGuid());

	public static ProficiencyLevelId From(Guid value) => new(value);
}