using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Bunsetsu.BunsetsuDependencyRelationship;

public sealed class BunsetsuDependencyRelationshipId : StronglyTypedId<Guid>
{
	private BunsetsuDependencyRelationshipId(Guid value) : base(value)
	{
	}

	public static BunsetsuDependencyRelationshipId Create() => new(Guid.NewGuid());

	public static BunsetsuDependencyRelationshipId From(Guid value) => new(value);
}