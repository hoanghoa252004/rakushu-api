using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Common.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Specifications;

public sealed class ActiveUserSpecification
	: ISpecification<User>
{
	public Result IsSatisfiedBy(User user)
	{
		return user.IsActive();
	}
}