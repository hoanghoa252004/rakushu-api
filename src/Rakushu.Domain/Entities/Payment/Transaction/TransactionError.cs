using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment.Transaction;

public static class TransactionError
{
	public static readonly Error InvalidCode =
		Error.Validation("TRANSACTION.INVALID_CODE", "Transaction code is invalid.");

	public static readonly Error InvalidTxnRef =
		Error.Validation("TRANSACTION.INVALID_TXN_REF", "Transaction reference is invalid.");

	public static readonly Error InvalidUrl =
		Error.Validation("TRANSACTION.INVALID_URL", "Transaction url is invalid.");

	public static readonly Error InvalidExpiration =
		Error.Validation("TRANSACTION.INVALID_EXPIRATION", "Transaction expiration is invalid.");

	public static readonly Error InvalidPaymentId =
		Error.Validation("TRANSACTION.INVALID_PAYMENT_ID", "Payment ID is invalid.");

	public static readonly Error InvalidProvider =
		Error.Validation("TRANSACTION.INVALID_PROVIDER", "The specified provider is not supported in this system. " +
			"Please try one of these: " + string.Join(", ", Enum.GetNames(typeof(Provider))));

	public static readonly Error InvalidAmount =
		Error.Validation("TRANSACTION.INVALID_AMOUNT", "Amount must be greater than 0.");

	public static readonly Error InvalidCurrency = 
		Error.Validation("TRANSACTION.INVALID_CURRENCY","The specified currency is not supported. Valid currency: " 
			+ string.Join(", ", Enum.GetNames<Currency>()) + ".");

	public static readonly Error InvalidStatus =
		Error.Validation("TRANSACTION.INVALID_STATUS", "Transaction status is invalid.");

	public static readonly Error InvalidStatusTransition =
	Error.Validation("TRANSACTION.INVALID_STATUS_TRANSITION", "Invalid status transition for transaction.");
}
