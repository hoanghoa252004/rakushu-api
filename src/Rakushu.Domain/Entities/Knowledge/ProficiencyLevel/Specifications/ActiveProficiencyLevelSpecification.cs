using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.ProficiencyLevel.Specifications;

public sealed class ActiveProficiencyLevelSpecification
	: ISpecification<ProficiencyLevel>
{
	public Result IsSatisfiedBy(ProficiencyLevel level)
	{
		return level.IsActive ? Result.Success() : Result.Failure(ProficiencyLevelErrors.NotActive);
	}
}