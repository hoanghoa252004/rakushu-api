using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Plan.PlanEntitlement;

public static class PlanEntitlementErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"PLAN_ENTITLEMENT.NOT_FOUND",
		"The specified plan entitlement was not found.");

	public static readonly Error DuplicateFeature = Error.Conflict(
		"PLAN_ENTITLEMENT.DUPLICATE_FEATURE",
		"An entitlement for this feature already exists in the plan.");

	public static readonly Error InvalidLimitValue = Error.Validation(
		"PLAN_ENTITLEMENT.INVALID_LIMIT_VALUE",
		"Limit value must be greater than or equal to 0.");

	public static readonly Error InvalidLimitUnit = Error.Validation(
		"PLAN_ENTITLEMENT.INVALID_LIMIT_UNIT",
		"The specified limit unit is invalid. Valid limit units: "
		+ string.Join(", ", Enum.GetNames<LimitUnit>()) + ".");

	public static readonly Error InvalidLimitPeriod = Error.Validation(
		"PLAN_ENTITLEMENT.INVALID_LIMIT_PERIOD",
		"The specified limit period is invalid. Valid limit periods: "
		+ string.Join(", ", Enum.GetNames<LimitPeriod>()) + ".");

	public static readonly Error PlanArchived = Error.Conflict(
		"PLAN_ENTITLEMENT.PLAN_ARCHIVED",
		"Cannot modify entitlements of an archived plan.");

	public static readonly Error FeatureNotActive = Error.Conflict(
		"PLAN_ENTITLEMENT.FEATURE_NOT_ACTIVE",
		"Only active features can be assigned to a plan.");

	public static readonly Error CannotDeleteEntitlementWithSubscriptions = Error.Conflict(
		"PLAN_ENTITLEMENT.CANNOT_DELETE_WITH_SUBSCRIPTIONS",
		"Cannot delete an entitlement from a plan that has active subscriptions. You can disable it instead.");
}
