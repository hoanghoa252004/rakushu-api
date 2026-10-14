using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.ContentCategory.Specifications;

public sealed class ActiveContentCategorySpecification
	: ISpecification<ContentCategory>
{
	public Result IsSatisfiedBy(ContentCategory category)
	{
		return category.IsActive
			? Result.Success()
			: Result.Failure(ContentCategoryErrors.NotActive);
	}
}
