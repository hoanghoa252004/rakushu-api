using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.RefreshToken;

public class RefreshTokenId : StronglyTypedId<Guid>
{
	public RefreshTokenId(Guid value) : base(value)
	{
	}

	public static RefreshTokenId Create() => new(Guid.NewGuid());
	public static RefreshTokenId From(Guid value) => new(value);
}
