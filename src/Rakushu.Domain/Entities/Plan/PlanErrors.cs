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

	public static Error NotFound() => Error.NotFound(
		"PLAN.NOT_FOUND", "The specified plan was not found.");

	public static Error DuplicateCode(string code) => Error.Conflict(
		"PLAN.DUPLICATE_CODE", $"A plan with code '{code}' already exists.");

	public static Error CannotDeletePlanWithSubscriptions() => Error.Conflict(
		"PLAN.CANNOT_DELETE", "Cannot delete a plan that has active subscriptions.");
}