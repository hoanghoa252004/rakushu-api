using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.User.Subscription;

public static class SubscriptionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SUBSCRIPTION.NOT_FOUND", "The subscription with the specified identifier was not found.");

	public static readonly Error AlreadyActive = Error.Conflict(
		"SUBSCRIPTION.ALREADY_ACTIVE", "The user already has an active subscription.");

	public static readonly Error CannotCancel = Error.Validation(
		"SUBSCRIPTION.CANNOT_CANCEL", "Subscription can only be canceled when it is currently Active.");

	public static readonly Error InvalidStatus = Error.Validation(
		"SUBSCRIPTION.INVALID_STATUS",
		"The specified subscription status is invalid. Only 'Canceled' is allowed.");
}
