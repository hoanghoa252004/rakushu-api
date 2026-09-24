using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Entities.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment;

public static class PaymentError
{
	public static readonly Error InvalidAmount =
		Error.Validation("PAYMENT.INVALID_AMOUNT", "Amount must be greater than 0.");

	public static readonly Error InvalidStatus =
		Error.Validation("PAYMENT.INVALID_STATUS", "Payment status is invalid. " +
			"Valid payment status: " + string.Join(", ", Enum.GetNames(typeof(PaymentStatus))));

	public static readonly Error InvalidPlanId =
		Error.Validation("PAYMENT.INVALID_PLAN_ID", "Plan ID is invalid.");

	public static readonly Error InvalidUserId =
		Error.Validation("PAYMENT.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error InvalidExpiration =
	Error.Validation("PAYMENT.INVALID_EXPIRATION", "Payment expiration is invalid.");

	public static readonly Error TransactionNotBelong =
	Error.Validation("PAYMENT.TRANSACTION_NOT_BELONG", "Transaction does not belong to the specified payment.");

	public static Error PaymentNotFoundForPlan(PlanId plandId) =>
		Error.NotFound("PAYMENT.NOT_FOUND_FOR_PLAN", $"Payment for plan ID '{plandId}' was not found.");

	public static readonly Error PaymentExpired =
		Error.Validation("PAYMENT.EXPIRED", "Payment has expired.");

	public static readonly Error InvalidCurrency =
		Error.Validation("PAYMENT.INVALID_CURRENCY", "The specified currency is not supported. Valid currency: "
			+ string.Join(", ", Enum.GetNames<Currency>()) + ".");

	public static readonly Error PaymentAlreadyPaid =
		Error.Validation("PAYMENT.ALREADY_PAID", "Payment has already been paid.");

	public static Error UnsupportedPaymentProvider(string provider) =>
		Error.Validation("PAYMENT.UNSUPPORT_PROVIDER", $"Payment provider '{provider}' is not supported.");

	public static readonly Error PendingTransactionExists =
		Error.Validation("PAYMENT.PENDING_TRANSACTION_EXISTS", "An pending transaction already exists. Please wait for it to expire before creating a new one.");

	public static readonly Error PlanNotFound =
		Error.NotFound("PAYMENT.PLAN_NOT_FOUND", "The specified plan was not found.");

	public static readonly Error PlanNotActive =
		Error.Validation("PAYMENT.PLAN_NOT_ACTIVE", "The specified plan is not active.");

	public static readonly Error ActiveSubscriptionExists =
		Error.Conflict("PAYMENT.ACTIVE_SUBSCRIPTION_EXISTS", "You may already have an active subscription in used currently. " +
			"If you want to subscribe / change to another one, please cancel the current subsription.");

	public static readonly Error ActiveSubscriptionWithSpecifiedPlanExists =
		Error.Conflict("PAYMENT.ACTIVE_SUBSCRIPTION_WITH_SPECIFIED_PLAN_EXISTS", "You already have an active subscription for this plan."); 

	public static readonly Error InvalidProvider =
		Error.Validation("PAYMENT.INVALID_PROVIDER", "The specified provider is invalid. " +
			"Valid provider: " + string.Join(", ", Enum.GetNames(typeof(Provider))));

	public static readonly Error PaymentNotFound =
		Error.NotFound("PAYMENT.NOT_FOUND", "The specified payment was not found.");

	public static readonly Error PaymentNotBelong =
		Error.Forbidden("PAYMENT.NOT_BELONG", "The specified payment does not belong to you.");

	public static readonly Error InvalidStatusTransition =
		Error.Validation("PAYMENT.INVALID_STATUS_TRANSITION", "Invalid status transition for payment.");

	public static readonly Error InvalidSignature =
		Error.Validation("PAYMENT.INVALID_SIGNATURE", "Invalid signature for payment.");

	//public static Error PaymentConfigNotFound() =>
	//	Error.NotFound("PAYMENT.PaymentConfigNotFound", "Payment configuration was not found.");

	//public static Error InvalidRange() =>
	//	Error.Validation("PAYMENT.InvalidRange", "Time must be > 0");

	//public static Error InvalidOnlinePaymentExpiredInDays() =>
	//	Error.Validation("PAYMENT.InvalidOnlinePaymentExpiredInDays", "Online payment expiration must be at least 1 and not > 10.");

	//public static Error InvalidOnlineTransactionExpiredInMinutes() =>
	//	Error.Validation("PAYMENT.InvalidOnlineTransactionExpiredInMinutes", "Online transaction expiration must be at least 5 minutes and not > 60 minutes.");
}
