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
		"PLAN.CANNOT_DELETE", "Cannot delete a plan that has been subscribed, you can only archive it.");
	public static readonly Error InvalidName = Error.Validation(
		"PLAN.INVALID_NAME",
		"Plan name is required and must not exceed 50 characters.");

	public static readonly Error InvalidPrice = Error.Validation(
		"PLAN.INVALID_PRICE",
		"Plan price must be greater than 0.");

	public static readonly Error InvalidCurrency = Error.Validation(
		"PLAN.INVALID_CURRENCY",
		"The specified currency is not supported.");

	public static readonly Error InvalidBillingCycle = Error.Validation(
		"PLAN.INVALID_BILLING_CYCLE",
		"The specified billing cycle is invalid.");

	public static readonly Error InvalidStatus = Error.Validation(
		"PLAN.INVALID_STATUS",
		"The specified plan status is invalid.");
}