using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;

public static class SubscriptionUsageErrors
{
	public static readonly Error AlreadyExpired = Error.Validation(
		"SUBSCRIPTION_USAGE.ALREADY_EXPIRED",
		"Subscription usage has already expired.");
	public static readonly Error AlreadyCanceled = Error.Validation(
		"SUBSCRIPTION_USAGE.ALREADY_CANCELED",
		"Subscription usage has already been canceled.");
}