using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

public sealed class DependencyRelationshipId : StronglyTypedId<Guid>
{
	private DependencyRelationshipId(Guid value) : base(value)
	{
	}

	public static DependencyRelationshipId Create() => new(Guid.NewGuid());

	public static DependencyRelationshipId From(Guid value) => new(value);
}