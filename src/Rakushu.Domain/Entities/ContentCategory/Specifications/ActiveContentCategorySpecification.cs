using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ContentCategory.Specifications;

public sealed class ActiveContentCategorySpecification
	: ISpecification<ContentCategory>
{
	public Result IsSatisfiedBy(ContentCategory category)
	{
		return category.Status == ContentCategoryStatus.Active
			? Result.Success()
			: Result.Failure(ContentCategoryErrors.NotActive);
	}
}
