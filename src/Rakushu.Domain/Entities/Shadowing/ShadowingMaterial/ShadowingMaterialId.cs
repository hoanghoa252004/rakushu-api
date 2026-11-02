using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public sealed class ShadowingMaterialId : StronglyTypedId<Guid>
{
	private ShadowingMaterialId(Guid value) : base(value)
	{
	}

	public static ShadowingMaterialId Create() => new(Guid.NewGuid());

	public static ShadowingMaterialId From(Guid value) => new(value);
}