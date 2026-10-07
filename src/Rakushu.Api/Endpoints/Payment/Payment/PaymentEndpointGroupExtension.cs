namespace Rakushu.Api.Endpoints.Payment.Payment;

internal static class PaymentEndpointGroupExtension
{
	internal static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/payments")
			.WithTags("Payment")
			.WithGroupName("payment");
	}
}
