using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

public sealed class TokenId : StronglyTypedId<Guid>
{
	private TokenId(Guid value) : base(value)
	{
	}

	public static TokenId Create() => new(Guid.NewGuid());

	public static TokenId From(Guid value) => new(value);
}