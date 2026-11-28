using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

public class PasswordResetCodeId : StronglyTypedId<Guid>
{
	private PasswordResetCodeId(Guid value) : base(value)
	{
	}

	public static PasswordResetCodeId Create() => new(Guid.NewGuid());
	public static PasswordResetCodeId From(Guid value) => new(value);
}
