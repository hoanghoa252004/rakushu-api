using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription;

public static class SubscriptionErrors
{
	public static readonly Error AlreadyExpired = Error.Validation(
		"SUBSCRIPTION.ALREADY_EXPIRED",
		"Subscription has already expired.");
	public static readonly Error AlreadyCanceled = Error.Validation(
		"SUBSCRIPTION.ALREADY_CANCELED",
		"Subscription has already been canceled.");
	public static readonly Error NotFound = Error.NotFound(
		"SUBSCRIPTION.NOT_FOUND",
		"Subscription not found.");
	public static readonly Error InvalidStatus = Error.Validation(
		"SUBSCRIPTION.INVALID_STATUS",
		"Invalid subscription status.");
	public static readonly Error InvalidUserId = Error.Validation(
		"SUBSCRIPTION.INVALID_USER_ID",
		"Invalid user ID.");
}
