using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public sealed class ShadowingMaterialRequestId : StronglyTypedId<Guid>
{
	private ShadowingMaterialRequestId(Guid value) : base(value)
	{
	}

	public static ShadowingMaterialRequestId Create() => new(Guid.NewGuid());

	public static ShadowingMaterialRequestId From(Guid value) => new(value);
}