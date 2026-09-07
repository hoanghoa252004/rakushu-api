using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Common.Errors;

public static class CommonError
{
	public static readonly Error InvalidStatusTransition = Error.Failure(
		"GENERAL.INVALID_STATUS_TRANSITION", "The status transition is invalid in the context.");
}
