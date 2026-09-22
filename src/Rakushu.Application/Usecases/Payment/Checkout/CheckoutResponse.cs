namespace Rakushu.Application.Usecases.Payment.Checkout;

public sealed record CheckoutResponse(
	Guid PaymentId,
	Guid SubscriptionId,
	string OrderCode,
	decimal Amount,
	string Currency,
	string? QrCodeUrl,
	DateTimeOffset ExpiresAt,
	string PlanName,
	string Bank,
	string AccountNumber
);
