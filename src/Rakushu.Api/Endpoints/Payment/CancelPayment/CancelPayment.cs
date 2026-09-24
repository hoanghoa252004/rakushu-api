using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.UpdatePaymentStatus;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.UpdatePaymentStatus;

internal sealed class CancelPayment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			// 1. Endpoint
			.MapPatch("/status/cancelled", async (
				[FromBody] UpdatePaymentStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CancelPaymentCommand(dto.PaymentId);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdatePaymentStatus")
			.WithDescription("Cancels the specified payment.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.Learner.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdatePaymentStatusRequestDto(Guid PaymentId);
