using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan;

public static class PlanErrors
{
	public static readonly Error InvalidCode = Error.Validation(
		"PLAN.INVALID_CODE", "The plan code is invalid, it must have value, not exceed 50 chars, all uppercase, and concat with '_'. For example: FREE_PLAN ; PREMIUM_PLAN.");
}