using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.Specifications;

public sealed class SameFrameworkSpecification
	: ISpecification<ProficiencyLevelPair>
{
	public Result IsSatisfiedBy(ProficiencyLevelPair candidate)
	{
		return candidate.Current.ProficiencyFrameworkId == candidate.Target.ProficiencyFrameworkId
			? Result.Success()
			: Result.Failure(ProficiencyLevelErrors.NotSameFramework);
	}
}
public sealed record ProficiencyLevelPair(
	ProficiencyLevel Current,
	ProficiencyLevel Target
);