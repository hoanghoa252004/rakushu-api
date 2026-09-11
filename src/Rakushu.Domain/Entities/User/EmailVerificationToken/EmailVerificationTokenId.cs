using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.EmailVerificationToken;

public class EmailVerificationTokenId : StronglyTypedId<Guid>
{
	private EmailVerificationTokenId(Guid value) : base(value)
	{
	}

	public static EmailVerificationTokenId Create() => new(Guid.NewGuid());
	public static EmailVerificationTokenId From(Guid value) => new(value);
}