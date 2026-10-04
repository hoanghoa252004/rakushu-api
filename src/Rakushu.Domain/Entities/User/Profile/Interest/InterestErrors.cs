using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Profile.Interest;

public static class InterestErrors
{
	public static readonly Error InvalidPriority = Error.Validation(
		"INTEREST.INVALID_PRIORITY", "Priority must be greater than 0.");
}