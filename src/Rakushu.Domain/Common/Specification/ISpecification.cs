using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Common.Specification;

internal interface ISpecification<in T>
{
	Result IsSatisfiedBy(T candidate);
}
