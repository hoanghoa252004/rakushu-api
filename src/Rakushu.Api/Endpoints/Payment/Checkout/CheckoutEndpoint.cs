using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.Checkout;

namespace Rakushu.Api.Endpoints.Payment.Checkout;

internal sealed class CheckoutEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapPaymentEndpoints();

		var handler = async (
			[FromBody] CheckoutRequestDto dto,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new CheckoutCommand(dto.PlanId);
			var result = await sender.Send(command, cancellationToken);

			return result.MatchCreated("GetPaymentById", response => new { id = response.PaymentId });
		};

		group.MapPost(string.Empty, handler)
			.WithName("CreatePayment")
			.WithDescription("Creates a payment order with SePay VietQR.")
			.RequireAuthorization()
			.Produces<CheckoutResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CheckoutRequestDto(Guid PlanId);
