using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

public sealed class ProficiencyEquivalenceId : StronglyTypedId<Guid>
{
	private ProficiencyEquivalenceId(Guid value) : base(value)
	{
	}

	public static ProficiencyEquivalenceId Create() => new(Guid.NewGuid());

	public static ProficiencyEquivalenceId From(Guid value) => new(value);
}