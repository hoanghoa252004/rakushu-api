using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ContentCategory.ContentProcessingPolicy;

public sealed class ContentProcessingPolicyId : StronglyTypedId<Guid>
{
	private ContentProcessingPolicyId(Guid value) : base(value)
	{
	}

	public static ContentProcessingPolicyId Create() => new(Guid.NewGuid());

	public static ContentProcessingPolicyId From(Guid value) => new(value);
}