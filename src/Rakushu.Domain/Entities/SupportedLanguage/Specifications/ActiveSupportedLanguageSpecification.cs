using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.SupportedLanguage.Specifications;

public class ActiveSupportedLanguageSpecification : ISpecification<SupportedLanguage>
{
	public Result IsSatisfiedBy(SupportedLanguage candidate)
	{
		return candidate.IsActive ? Result.Success() : Result.Failure(SupportedLanguageErrors.NotActive);
	}
}
