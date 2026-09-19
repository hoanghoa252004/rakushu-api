using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Domain.Entities.Feature;

public static class FeatureErrors
{
	public static readonly Error InvalidName = Error.Validation(
		"FEATURE.INVALID_NAME",
		"Feature name is required and must not exceed 50 characters.");
	public static readonly Error InvalidCode = Error.Validation(
		"FEATURE.INVALID_CODE", "The feature code is invalid, it must have value, not exceed 50 chars, all uppercase, and concat with '_'. For example: AI_CHAT ; QUIZ_GENERATION.");

	public static readonly Error NotFound = Error.NotFound(
		"FEATURE.NOT_FOUND", "The feature was not found.");

	public static readonly Error DuplicateCode = Error.Conflict(
		"FEATURE.DUPLICATE_CODE", "A feature with this code already exists.");

	public static readonly Error CannotDeleteFeatureWithSubscriptionUsage = Error.Conflict(
		"FEATURE.CANNOT_DELETE_WITH_SUBSCRIPTION_USAGE", "Cannot delete a feature that has subscription usage. You can only archived it.");

	public static readonly Error CannotDeleteFeatureHasBeenAtachedToAPlan = Error.Conflict(
		"FEATURE.CANNOT_DELETE_WITH_PLAN", "Cannot delete a feature that has been attached to a plan. Please detach from plan first.");

	public static readonly Error InvalidStatus = Error.Validation(
		"FEATURE.INVALID_STATUS",
		"The specified feature status is invalid. Valid feature status: "
		+ FeatureStatusTransition.GetFeatureStatuses() + ".");
}