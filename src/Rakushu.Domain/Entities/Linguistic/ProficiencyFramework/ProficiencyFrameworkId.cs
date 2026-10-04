using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;

public sealed class ProficiencyFrameworkId : StronglyTypedId<Guid>
{
	private ProficiencyFrameworkId(Guid value) : base(value)
	{
	}

	public static ProficiencyFrameworkId Create() => new(Guid.NewGuid());

	public static ProficiencyFrameworkId From(Guid value) => new(value);
}