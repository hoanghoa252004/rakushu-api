using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.ValueObjects.Profile;

public static class ProfileErrors
{
	public static readonly Error InvalidFullName = Error.Validation(
		"USER.PROFILE.INVALID_FULLNAME", "FullName is required & cannot exceed 50 characters.");
}
