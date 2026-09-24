using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Payment;

public static class PaymentErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"PAYMENT.NOT_FOUND", "The payment with the specified identifier was not found.");

	public static readonly Error OrderCodeNotFound = Error.NotFound(
		"PAYMENT.ORDER_CODE_NOT_FOUND", "Payment with the specified order code was not found.");

	public static readonly Error AlreadyProcessed = Error.Conflict(
		"PAYMENT.ALREADY_PROCESSED", "Payment has already been completed or processed.");

	public static readonly Error Expired = Error.Validation(
		"PAYMENT.EXPIRED", "Payment has expired.");

	public static readonly Error CannotCancel = Error.Validation(
		"PAYMENT.CANNOT_CANCEL", "Payment can only be canceled when it is in Pending state.");

	public static readonly Error InvalidAmount = Error.Validation(
		"PAYMENT.INVALID_AMOUNT", "Payment amount must be greater than zero.");

	public static readonly Error AmountMismatch = Error.Validation(
		"PAYMENT.AMOUNT_MISMATCH", "Transfer amount is less than the required payment amount.");

	public static readonly Error DuplicateSepayTransaction = Error.Conflict(
		"PAYMENT.DUPLICATE_SEPAY_TRANSACTION", "This transaction has already been processed.");

	public static readonly Error TransactionNotFound = Error.NotFound(
		"PAYMENT.TRANSACTION_NOT_FOUND", "The payment transaction was not found.");

	public static readonly Error TransactionCannotBeCanceled = Error.Validation(
		"PAYMENT.TRANSACTION_CANNOT_BE_CANCELED", "Payment transaction can only be canceled when it is in Pending state.");
}
