using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.CreatePayment;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.CreatePayment;

internal sealed class CreatePayment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreatePaymentRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreatePaymentCommand(
					dto.PlanId);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetPaymentById", paymentId => new { id = paymentId });
			})
			// 2. Description
			.WithName("CreatePayment")
			.WithDescription("Creates a new payment for the specified plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.Learner.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreatePaymentRequestDto(
	Guid PlanId
);
